using CommonObjects.Requests.Files;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Models;

public class UserAvatarService(
    UserInfo userInfo,
    IFileService fileService,
    IFileApiService fileApiService,
    IFingerprintService fingerprintService,
    IUserAvatarApiService userAvatarApiService) : IUserAvatarService
{
    private readonly IFileService _fileService = fileService;
    private readonly IFileApiService _fileApiService = fileApiService;
    private readonly IUserAvatarApiService _userAvatarApiService = userAvatarApiService;
    private readonly IFingerprintService _fingerprintService = fingerprintService;
    private readonly UserInfo _userInfo = userInfo;

    public async Task<Result<AvatarResponse>> GetToken(byte[] accessToken, CancellationToken cancellationToken = default)
    {
        return await _userAvatarApiService.Get(accessToken, cancellationToken);
    }

    public async Task<Result> Set()
    {
        Result<FileObject> fileResult = _fileService.GetImage();
        if (!fileResult.Success)
            return Result.From(fileResult);

        FingerprintExistL3 l3 = await _fingerprintService.GetFingerPrintFull(fileResult.Body!.Path!, default);
        Result<byte[]> tokenResult = await _fileApiService.ExistsL3(l3);

        if (!tokenResult.Success)
        {
            MultipartFormDataContent? data = _fileService.GetStream(fileResult.Body!.Path ?? "");
            if (data is null)
                return Result.BadRequest("Invalid file");

            tokenResult = await _fileApiService.Save(data);
            if (!tokenResult.Success)
                return Result.From(tokenResult);
        }

        Result result = await _userAvatarApiService.Set(new(new(
                tokenResult.Body!,
                [],
                fileResult.Body.Name ?? "",
                fileResult.Body.Size)));

        if(result.Success)
            _userInfo.User.Avatar = new() 
            {
                Path = fileResult.Body.Path
            };

        return result;
    }
}
