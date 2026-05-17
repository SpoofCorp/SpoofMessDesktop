using CommonObjects.Requests.Files;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using System.IO;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Models;

public class ChatAvatarService(
    IFileService fileService,
    IFileApiService fileApiService,
    IFingerprintService fingerprintService,
    INotificationService notificationService,
    IChatAvatarApiService chatAvatarApiService,
    UserInfo userInfo) : IChatAvatarService
{
    private readonly IFileService _fileService = fileService;
    private readonly IFileApiService _fileApiService = fileApiService;
    private readonly IChatAvatarApiService _chatAvatarApiService = chatAvatarApiService;
    private readonly UserInfo _userInfo = userInfo;
    private readonly IFingerprintService _fingerprintService = fingerprintService;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result> Set(Chat chat, CancellationToken cancellationToken = default)
    {
        Result<FileObject> fileResult = _fileService.GetImage();
        if (!fileResult.Success)
            return Result.From(fileResult);

        FingerprintExistL3 l3 = await _fingerprintService.GetFingerPrintFull(fileResult.Body!.Path!, cancellationToken);
        Result<byte[]> tokenResult = await _fileApiService.ExistsL3(l3, cancellationToken);

        if (!tokenResult.Success)
        {
            MultipartFormDataContent? data = _fileService.GetStream(fileResult.Body!.Path ?? "");
            if (data is null)
                return Result.BadRequest("Invalid file");

            tokenResult = await _fileApiService.Save(data, cancellationToken);
            if (!tokenResult.Success)
                return Result.From(tokenResult);
        }

        Result result = await _chatAvatarApiService.Set(new(new(
                tokenResult.Body!,
                [],
                fileResult.Body.Name ?? "",
                fileResult.Body.Size),
                chat.Id), cancellationToken);

        if (result.Success)
            chat.Avatar = new()
            {
                Path = fileResult.Body.Path
            };
        else
            _notificationService.ShowError(result.Error);

        return result;
    }

    public async Task UploadAvatar(Chat chat, CancellationToken cancellationToken = default)
    {
        await _fileService.Save(chat.Avatar, cancellationToken);
    }

    public async Task OnChatAvatarUpdated(FileObject avatar, CancellationToken cancellationToken = default)
    {
        Result<AvatarResponse> result = await _chatAvatarApiService.Get(avatar.AttachmentToken!, cancellationToken);
        if (!result.Success)
            return;
        avatar.Path = Path.Combine(_userInfo.SessionSettings.Directory, result.Body!.FileMetadata.OriginalName ?? "");
        avatar.Token = result.Body.FileMetadata.Token;
        avatar.Id = result.Body.FileMetadata.Id;
        await _fileService.Save(avatar, cancellationToken);
    }
}
