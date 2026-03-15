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
    }

    private async Task UploadAttachment(FileObject file, MessageModel message)
    {
        Result<Stream> result = await _fileApiService.Upload(file.Token!);
        if (result.Success)
        {
            file.Path = _userInfo.SessionSettings.Directory;

            await _fileService.Save(result.Body!, file); 
            App.Current.Dispatcher.Invoke(() =>
            {
                message.Attachments.Add(GetViewModel(file));
            });
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

    public Result Attach(MessageModel message)
    {
        Result<FileObject> file = _fileService.GetFileInfo();
        if (!file.Success)
            return Result.From(file);

        message.Attachments.Add(GetViewModel(file.Body!));
        return Result.OkResult();
    }

    [Obsolete("Need remove file from server")]
    public void Unattach(FileObject file, MessageModel message)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            message.Attachments.Remove(GetViewModel(file));
        });
    }

    public async Task<Result<byte[]>> SendAttachment(FileObject file)
    {
        Result<byte[]> resultL3;
        if (file.Size < 50 * 1024 * 1024)
        {
            FingerprintExistL3 l3 = await _fingerprintService.GetFingerPrintFull(file.Path!);
            resultL3 = await _fileApiService.ExistsL3(l3);
            if (resultL3.Success)
                return resultL3;
            else if (resultL3.StatusCode == 404)
                return await Save(file);
            else return resultL3;
        }
        else
        {
            FingerprintExistL1L2 l1 = await _fingerprintService.GetFingerPrintL1L2(file.Path!);
            Result result = await _fileApiService.ExistsL1L2(l1);
            if (result.Success)
                return await Save(file);

            FingerprintExistL3 l3 = await _fingerprintService.GetFingerPrintFull(file.Path!);
            resultL3 = await _fileApiService.ExistsL3(l3);
            if (!resultL3.Success)
                return await Save(file);
            return resultL3;

        }
    }
    private async Task<Result<byte[]>> Save(FileObject file)
    {
        using MultipartFormDataContent form = _fileService.GetStream(file.Path!);
        return await _fileApiService.Save(form);
    }
}
