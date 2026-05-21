using AdditionalHelpers.Services;
using CommonObjects.Requests.Attachments;
using CommonObjects.Requests.Files;
using CommonObjects.Results;
using SpoofFileParser.FileMetadata;
using SpoofMess.Enums;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.ViewModels.FileViewModels;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace SpoofMess.ServiceRealizations.Models;

internal class AttachmentService(
    IFileService fileService,
    IFingerprintService fingerprintService,
    IFileApiService fileApiService,
    INavigationService navigationService,
    ISerializer serializer,
    IAttachmentApiService attachmentApiService,
    UserInfo userInfo) : IAttachmentService
{
    private readonly INavigationService _navigationService = navigationService;
    private readonly IAttachmentApiService _attachmentApiService = attachmentApiService;
    private readonly UserInfo _userInfo = userInfo;
    private readonly ISerializer _serializer = serializer;
    private readonly IFileService _fileService = fileService;
    private readonly IFileApiService _fileApiService = fileApiService;
    private readonly IFingerprintService _fingerprintService = fingerprintService;

    public async Task UploadAttachments(MessageModel message, List<Attachment> attachments)
    {
        if (attachments is null || attachments.Count == 0)
            return;
        ConcurrentBag<FileObject> files = [];
        await Parallel.ForEachAsync(attachments, async (attachment, cancellationToken) =>
        {
            Result<CommonObjects.DTO.FileMetadata> result = await _attachmentApiService.GetToken(attachment.Token);
            if (!result.Success)
            {
                //need handle error
                return;
            }
            FileObject file = new()
            {
                Id = attachment.Id,
                AttachmentToken = attachment.Token,
                Token = result.Body!.Token,
                Category = Enum.Parse<FileCategory>(attachment.Category, true),
                Name = attachment.OriginalFileName,
                Path = Path.Combine(_userInfo.SessionSettings.Directory, attachment.OriginalFileName),
                Metadata = _serializer.Deserialize<IFileMetadata>(attachment.Metadata ?? "")
            };
            file.Size = file.Metadata.Size;
            file.PrettySize = _fileService.ToPrettySize(file.Metadata.Size);
            await UploadAttachment(file, files);
        });
        files = [.. files.OrderByDescending(x => (int)x.Category).ThenBy(x => x.Id, Comparer<byte[]?>.Create(StructuralComparisons.StructuralComparer.Compare))];
        Application.Current.Dispatcher.Invoke(() =>
        {
            foreach (var file in files)
                Add(file, message);
        });
        GC.Collect();
    }

    private async Task UploadAttachment(FileObject file, ConcurrentBag<FileObject> files)
    {
        files.Add(file);

        try
        {
            await _fileService.Save(file);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public ObjectViewModel GetViewModel(FileObject file) =>
        file.Category switch
        {
            FileCategory.Image => _navigationService.GetImageViewModel(file),
            FileCategory.Audio => _navigationService.GetMusicViewModel(file),
            _ => _navigationService.GetFileViewModel(file),
        };

    private void Add(FileObject file, MessageModel message)
    {
        ObjectViewModel fileView = GetViewModel(file);
        ObjectViewModel? vm = message.Attachments.FirstOrDefault(x => x.GetType() == fileView.GetType());

        if (message is EditMessageModel editMessage)
            editMessage.NewAttachments.Add(new(true, file));
        if (vm is not null)
            vm.Add(file);
        else
            message.Attachments.Add(fileView);
    }

    public Result Attach(MessageModel message)
    {
        Result<List<FileObject>> files = _fileService.GetFilesInfo();
        if (!files.Success)
            return Result.From(files);

        foreach (FileObject file in files.Body!)
            Add(file, message);
        return Result.OkResult();
    }

    public void Unattach(FileObject file, MessageModel message)
    {
        ObjectViewModel fileView = GetViewModel(file);
        var vm = message.Attachments.FirstOrDefault(x => x.GetType() == fileView.GetType());
        if (vm is not null)
        {
            FileObject? model = vm.Files.FirstOrDefault(x => x == file);
            if (model is not null)
            {
                if (message is EditMessageModel editMessage)
                    editMessage.NewAttachments.Add(new(false, model));
                Application.Current.Dispatcher.Invoke(() =>
                {
                    vm.Files.Remove(model);
                });
            }
        }
    }

    public async Task<Result<List<Attachment>>> SendAttachments(MessageModel message, CancellationToken token = default)
    {
        ParallelOptions options = new()
        {
            CancellationToken = new(),
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2)
        };
        ConcurrentBag<Attachment> attachments = [];
        await Parallel.ForEachAsync(message.Attachments.SelectMany(x => x.Files), options, async (file, ct) =>
        {
            Result<byte[]> accessFileToken = await SendAttachment(file, token);
            if (accessFileToken.Success)
                attachments.Add(new(file.Id ?? [], accessFileToken.Body!, file.Name!, string.Empty, string.Empty, file.Size));
        });
        return Result<List<Attachment>>.OkResult([.. attachments]);
    }

    public async Task<Result<List<CommonObjects.Responses.EditAttachment>>> SendAttachments(EditMessageModel message, CancellationToken token = default)
    {
        ParallelOptions options = new()
        {
            CancellationToken = new(),
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2)
        };
        ConcurrentBag<CommonObjects.Responses.EditAttachment> attachments = [];
        foreach (EditAttachment editAttachment in message.NewAttachments.Where(x => !x.IsAdded))
            attachments.Add(new(false, editAttachment.FileObject.Id ?? [], editAttachment.FileObject.AttachmentToken!, editAttachment.FileObject.Name!, string.Empty, string.Empty, editAttachment.FileObject.Size));

        await Parallel.ForEachAsync(message.NewAttachments.Where(x => x.IsAdded), options, async (file, ct) =>
        {
            Result<byte[]> accessFileToken = await SendAttachment(file.FileObject, token);
            if (accessFileToken.Success)
                attachments.Add(new(file.IsAdded, file.FileObject.Id ?? [], accessFileToken.Body!, file.FileObject.Name!, string.Empty, string.Empty, file.FileObject.Size));
        });
        return Result<List<CommonObjects.Responses.EditAttachment>>.OkResult([.. attachments]);
    }

    public async Task<Result<byte[]>> SendAttachment(FileObject file, CancellationToken token = default)
    {
        Result<byte[]> resultL3;
        if (file.Size < 50 * 1024 * 1024)
        {
            FingerprintExistL3 l3 = await _fingerprintService.GetFingerPrintFull(file.Path!, token);
            resultL3 = await _fileApiService.ExistsL3(l3, token);
            if (resultL3.Success)
                return resultL3;
            else if (resultL3.StatusCode == 404)
                return await Save(file, token);
            else return resultL3;
        }
        else
        {
            FingerprintExistL1L2 l1 = _fingerprintService.GetFingerPrintL1L2(file.Path!);
            Result result = await _fileApiService.ExistsL1L2(l1, token);
            if (result.Success)
                return await Save(file, token);

            FingerprintExistL3 l3 = await _fingerprintService.GetFingerPrintFull(file.Path!, token);
            resultL3 = await _fileApiService.ExistsL3(l3, token);
            if (!resultL3.Success)
                return await Save(file, token);
            return resultL3;

        }
    }
    private async Task<Result<byte[]>> Save(FileObject file, CancellationToken token = default)
    {
        using MultipartFormDataContent form = _fileService.GetStream(file.Path!);
        return await _fileApiService.Save(form, token);
    }

    public async Task UploadAttachments(MessageModel message, List<CommonObjects.Responses.EditAttachment> attachments)
    {
        ConcurrentBag<FileObject> files = [];
        await Parallel.ForEachAsync(attachments, async (attachment, cancellationToken) =>
        {
            if (attachment.IsAdded)
            {
                Result<CommonObjects.DTO.FileMetadata> result = await _attachmentApiService.GetToken(attachment.Token);
                if (!result.Success)
                {
                    //need handle error
                    return;
                }
                FileObject file = new()
                {
                    Id = attachment.Id,
                    AttachmentToken = attachment.Token,
                    Token = result.Body!.Token,
                    Category = Enum.Parse<FileCategory>(attachment.Category, true),
                    Name = attachment.OriginalFileName,
                    Path = Path.Combine(_userInfo.SessionSettings.Directory, attachment.OriginalFileName),
                    Metadata = _serializer.Deserialize<IFileMetadata>(attachment.Metadata ?? "")
                };
                file.Size = file.Metadata.Size;
                file.PrettySize = _fileService.ToPrettySize(file.Metadata.Size);
                await UploadAttachment(file, files);
            }
            else
            {
                FileObject? file = null;
                ObjectViewModel? viewModel = null;

                foreach(ObjectViewModel objectViewModel in message.Attachments)
                {
                    file = objectViewModel.Files.FirstOrDefault(x => x.Id.SequenceEqual(attachment.Id));
                    if (file is not null)
                    {
                        viewModel = objectViewModel;
                        break;
                    }
                }
                if (file is null || viewModel is null)
                    return;

                _fileService.Remove(file);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    viewModel.Files.Remove(file);
                });
            }
        });
        files = [.. files.OrderByDescending(x => (int)x.Category).ThenBy(x => x.Id, Comparer<byte[]?>.Create(StructuralComparisons.StructuralComparer.Compare))];
        Application.Current.Dispatcher.Invoke(() =>
        {
            ObjectViewModel? vm;
            foreach (var file in files)
            {
                vm = Get(message, file);
                if (vm is null)
                    continue;
                vm.Add(file);
            }
        });

        ObjectViewModel? Get(MessageModel message, FileObject file)
        {
            ObjectViewModel fileView = GetViewModel(file);
            return message.Attachments.FirstOrDefault(x => x.GetType() == fileView.GetType()) ?? fileView;
        }
    }
}
