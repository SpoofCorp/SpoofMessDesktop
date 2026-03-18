using CommonObjects.Requests.Attachments;
using CommonObjects.Requests.Files;
using CommonObjects.Results;
using SpoofMess.Enums;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.ViewModels.FileViewModels;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace SpoofMess.ServiceRealizations.Models;

internal class AttachmentService(
    IFileService fileService,
    IFingerprintService fingerprintService,
    IFileApiService fileApiService,
    INavigationService navigationService,
    UserInfo userInfo) : IAttachmentService
{
    private readonly INavigationService _navigationService = navigationService;
    private readonly UserInfo _userInfo = userInfo;
    private readonly IFileService _fileService = fileService;
    private readonly IFileApiService _fileApiService = fileApiService;
    private readonly IFingerprintService _fingerprintService = fingerprintService;

    public async Task UploadAttachments(MessageModel message, List<Attachment> attachments)
    {
        await Parallel.ForEachAsync(attachments, async (attachment, cancellationToken) =>
        {
            FileObject file = new()
            {
                Token = attachment.Token,
                Category = _fileService.GetCategory(attachment),
                Name = attachment.OriginalFileName,
                Size = attachment.Size,
                Path = _userInfo.SessionSettings.Directory,
                PrettySize = _fileService.ToPrettySize(attachment.Size),

            };
            await UploadAttachment(file, message);
        });
        GC.Collect();
    }

    private async Task UploadAttachment(FileObject file, MessageModel message)
    {
        Result<Stream> result = await _fileApiService.Upload(file.Token!);
        if (result.Success)
        {
            file.Path = _userInfo.SessionSettings.Directory;
            App.Current.Dispatcher.Invoke(() =>
            {
                Add(file, message);
            });

            await _fileService.Save(result.Body!, file);
        }
        else
        {
            Debug.WriteLine("Пизда");
        }
    }

    public FileViewModel GetViewModel(FileObject file) =>
        file.Category switch
        {
            FileCategory.Image => _navigationService.GetImageViewModel(file),
            FileCategory.Audio => _navigationService.GetMusicViewModel(file),
            _ => _navigationService.GetFileViewModel(file),
        };

    private void Add(FileObject file, MessageModel message)
    {
        FileViewModel fileView = GetViewModel(file);
        FileViewModel? vm = message.Attachments.FirstOrDefault(x => x.GetType() == fileView.GetType());
        if (vm is not null)
            vm.Files.Add(file);
        else
            message.Attachments.Add(fileView);
    }

    public Result Attach(MessageModel message)
    {
        Result<FileObject> file = _fileService.GetFileInfo();
        if (!file.Success)
            return Result.From(file);

        Add(file.Body!, message);
        return Result.OkResult();
    }

    [Obsolete("Need remove file from server")]
    public void Unattach(FileObject file, MessageModel message)
    {
        FileViewModel fileView = GetViewModel(file);
        var vm = message.Attachments.FirstOrDefault(x => x.GetType() == fileView.GetType());
        if (vm is not null)
        {
            FileObject? model = vm.Files.FirstOrDefault(x => x == file);
            if (model is not null)
                App.Current.Dispatcher.Invoke(() =>
                {
                    vm.Files.Remove(model);
                });
        }
    }

    public async Task<Result<List<Attachment>>> SendAttachments(MessageModel message, CancellationToken token = default)
    {
        ParallelOptions options = new()
        {
            CancellationToken = new(),
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2)
        };
        List<Attachment> attachments = [];
        await Parallel.ForEachAsync(message.Attachments.SelectMany(x => x.Files), options, async (file, ct) =>
        {
            Result<byte[]> accessFileToken = await SendAttachment(file, token);
            if (accessFileToken.Success)
                attachments.Add(new(accessFileToken.Body!, file.Name!, string.Empty, file.Size));
        });
        return Result<List<Attachment>>.OkResult(attachments);
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
                return await Save(file);
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
}
