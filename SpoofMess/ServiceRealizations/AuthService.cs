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
using System.Text.Json;

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

    public async Task<Result> Initialize(CancellationToken cancellationToken = default)
    {
        Stream? stream = _fileService.GetReadStreamFromFile(BackupPath);
        if (stream is null)
            return Result.ErrorResult("Stream is null");
        UserInfo userInfo;
        try
        {
            userInfo = await _serializer.Deserialize<UserInfo>(stream, cancellationToken);
        }
        catch(JsonException)
        {
            return Result.ErrorResult("Session data has broken");
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
        catch (Exception ex)
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
        await Save(cancellationToken);
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
        await Save(cancellationToken);
    }

    public async Task Save(CancellationToken cancellationToken = default)
    {
        await fileSemaphore.WaitAsync(cancellationToken);
        try
        {
            string path = Path.GetDirectoryName(BackupPath)!;
            string tempPath = $"{BackupPath}.tmp";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            Stream? stream = null;
            try
            {
                stream = _fileService.GetWriteStreamFromFile(tempPath);
                if (stream is null)
                    return;
                await _serializer.Serialize(
                    UserInfo,
                    stream,
                    cancellationToken
                );
                await stream.FlushAsync(cancellationToken);
                await stream.DisposeAsync();
                stream = null;
                if (File.Exists(tempPath))
                    File.Move(tempPath, BackupPath, true);
            }
            finally
            {
                if (stream is not null)
                    await stream.DisposeAsync();
            }
        }
        finally
        {
            fileSemaphore.Release();
        }
    }
}