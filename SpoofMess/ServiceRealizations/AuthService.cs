using AdditionalHelpers.Services;
using CommonObjects.Requests;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.ServiceRealizations.Api;
using SpoofMess.Services;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations;

public class AuthService(
    UserInfo userInfo,
    HttpClient client,
    IFileService fileService,
    ISerializer serializer
    ) : ApiService(
        client,
        serializer
        ), IAuthService
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private DateTime ValidTo;
    protected override string BaseUrl => "https://localhost:7217/api/v1/Entrance";
    private string BackupPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SessionData.json");
    private readonly IFileService _fileService = fileService;
    private readonly UserInfo UserInfo = userInfo;

    public async Task<bool> Initialize()
    {
        Stream? stream = _fileService.GetObjectFromFile(BackupPath);
        if (stream is null)
            return false;
        UserInfo userInfo = await _serializer.Deserialize<UserInfo>(stream);
        stream.Dispose();
        UserInfo.Update(userInfo);
        if (UserInfo.Authorize is null)
            return false;

        return await Update();
    }

    public async Task<string?> GetAccess()
    {
        if (TokenIsNotExpired())
            return UserInfo.Authorize!.AccessToken;
        else if (UserInfo.Authorize is null)
            return null;

        await _semaphore.WaitAsync();
        try
        {
            if (TokenIsNotExpired()) return UserInfo.Authorize.AccessToken;
            await Update();
            return UserInfo.Authorize.AccessToken;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("", ex);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<bool> Update()
    {
        Result<UserAuthorizeResponse> result = await PostAsync<UpdateTokenRequest, UserAuthorizeResponse>(
            "/UpdateToken",
            new UpdateTokenRequest()
            {
                Token = UserInfo.Authorize!.RefreshToken
            });
        if (result.Success)
            SetTokens(result.Body!);

        return result.Success;
    }

    public void SetTokens(UserAuthorizeResponse response)
    {
        UserInfo.Authorize = response;
        File.WriteAllText(BackupPath, _serializer.Serialize(UserInfo));

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
}