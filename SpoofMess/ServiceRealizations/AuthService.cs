using AdditionalHelpers.Services;
using CommonObjects.Requests;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Bases;
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
    private static readonly SemaphoreSlim fileSemaphore = new(1, 1);
    private static string BackupPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpoofMess", "SessionData.json");

    public bool IsAuthorized { get; set; }
    public bool IsInitialized { get; set; }

    private readonly IFileService _fileService = fileService;
    private readonly UserInfo UserInfo = userInfo;

    public async Task<Result> Initialize()
    {
        Stream? stream = _fileService.GetReadStreamFromFile(BackupPath);
        if (stream is null)
            return Result.ErrorResult("Stream is null");
        UserInfo userInfo;
        try
        {
            userInfo = await _serializer.Deserialize<UserInfo>(stream);
        }
        finally
        {
            await stream.DisposeAsync();
        }
        UserInfo.Init(userInfo);
        if (UserInfo.Authorize is null)
            return Result.ErrorResult("Empty authorize data");

        return await Update();
    }

    public async Task<string?> GetAccess()
    {
        if (TokenIsNotExpired())
            return UserInfo.Authorize!.AccessToken;
        else if (UserInfo.Authorize is null)
            return null;

        await semaphore.WaitAsync();
        try
        {
            if (TokenIsNotExpired()) return UserInfo.Authorize.AccessToken;
            await Update();
            return UserInfo.Authorize.AccessToken;
        }
        catch (Exception ex)
        {
            return null;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<Result> Update()
    {
        Result<UserAuthorizeResponse> result = await PostAsync<UpdateTokenRequest, UserAuthorizeResponse>(
           "api/v1/Entrance/UpdateToken",
           new UpdateTokenRequest()
           {
               Token = UserInfo.Authorize!.RefreshToken
           });
        if (result)
        {
            await SetTokens(result.Body!);
            IsAuthorized = true;
        }

        return Result.From(result);
    }

    public async Task SetTokens(UserAuthorizeResponse response)
    {
        UserInfo.Authorize = response;
        await Save();
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

    public async Task Update(UserInfoUpdateModel changes)
    {
        UserInfo.Update(changes);
        await Save();
    }

    public async Task Save()
    {
        await fileSemaphore.WaitAsync();
        try
        {
            string path = Path.GetDirectoryName(BackupPath)!;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            Stream? stream = _fileService.GetWriteStreamFromFile(BackupPath);
            if (stream is null)
                return;
            try
            {
                await _serializer.Serialize(UserInfo, stream);
                await stream.FlushAsync();
            }
            finally
            {
                await stream.DisposeAsync();
            }
        }
        finally
        {
            fileSemaphore.Release();
        }
    }
}