using AdditionalHelpers.Services;
using CommonObjects.Requests;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Models.UpdateModels;
using SpoofMess.ServiceRealizations.Api;
using SpoofMess.Services;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations;

public class AuthService(
    UserInfo userInfo,
    AppSessionSettings appSessionSettings,
    HttpClient client,
    IFileService fileService,
    ISerializer serializer
    ) : ApiService(
        client,
        serializer
        ), IAuthService
{
    private DateTime ValidTo;
    private static readonly SemaphoreSlim semaphore = new(1, 1);
    public static string BackupPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpoofMess");
    public static string BackupSessionDataPath => Path.Combine(BackupPath, "SessionData.json");
    public static string BackupAppSessionSettingsPath => Path.Combine(BackupPath, "AppSessionSettings.json");

    public bool IsAuthorized { get; set; }
    public bool IsInitialized { get; set; }

    private readonly IFileService _fileService = fileService;
    private readonly UserInfo UserInfo = userInfo;
    private readonly AppSessionSettings _appSessionSettings = appSessionSettings;

    public async Task<Result> Initialize(CancellationToken cancellationToken = default)
    {
        Result<UserInfo> userInfo = await _fileService.GetFromFile<UserInfo>(BackupSessionDataPath, cancellationToken);
        if (userInfo)
            UserInfo.Init(userInfo.Body!);
        if (UserInfo.Authorize is null)
            return Result.ErrorResult("Empty authorize data");
        return await Update(cancellationToken);
    }

    public async Task<string?> GetAccess(CancellationToken cancellationToken = default)
    {
        if (TokenIsNotExpired())
            return UserInfo.Authorize!.AccessToken;
        else if (UserInfo.Authorize is null)
            return null;

        await semaphore.WaitAsync(cancellationToken);
        try
        {
            if (TokenIsNotExpired()) return UserInfo.Authorize.AccessToken;
            await Update(cancellationToken);
            return UserInfo.Authorize.AccessToken;
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<Result> Update(CancellationToken cancellationToken = default)
    {
        Result<UserAuthorizeResponse> result = await PostAsync<UpdateTokenRequest, UserAuthorizeResponse>(
           "api/v1/Entrance/UpdateToken",
           new UpdateTokenRequest()
           {
               Token = UserInfo.Authorize!.RefreshToken
           },
           cancellationToken
        );
        if (result)
        {
            await SetTokens(result.Body!, cancellationToken);
            IsAuthorized = true;
        }

        return Result.From(result);
    }

    public async Task SetTokens(UserAuthorizeResponse response, CancellationToken cancellationToken = default)
    {
        UserInfo.Authorize = response;
        await SaveUserInfo(cancellationToken);
        JwtSecurityTokenHandler jwt = new();
        JwtSecurityToken token = jwt.ReadJwtToken(UserInfo.Authorize.AccessToken);
        ValidTo = token.ValidTo;
    }

    protected bool TokenIsNotExpired()
    {
        if (UserInfo.Authorize is null)
            return false;

        return ValidTo >= DateTime.UtcNow.AddSeconds(15);
    }

    public async Task Update(UserInfoUpdateModel changes, CancellationToken cancellationToken = default)
    {
        UserInfo.Update(changes);
        await SaveUserInfo(cancellationToken);
    }

    public async Task SaveUserInfo(CancellationToken cancellationToken = default)
    {
        await _fileService.SaveToFile(BackupSessionDataPath, UserInfo, cancellationToken);
    }

    public async Task SaveAppSessionSettings(CancellationToken cancellationToken = default)
    {
        await _fileService.SaveToFile(BackupAppSessionSettingsPath, _appSessionSettings, cancellationToken);
    }
}