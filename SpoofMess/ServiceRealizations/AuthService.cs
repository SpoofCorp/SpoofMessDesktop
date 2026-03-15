using AdditionalHelpers.Services;
using CommonObjects.Requests;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.ServiceRealizations.Api;
using SpoofMess.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations;

public class AuthService(
    HttpClient client,
    ISerializer serializer
    ) : ApiService(
        client,
        serializer
        ), IAuthService
{
    private UserAuthorizeResponse? SessionData;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private DateTime ValidTo;
    protected override string BaseUrl => "https://localhost:7217/api/v1/Entrance";

    public async Task<string?> GetAccess()
    {
        if (TokenIsNotExpired())
            return SessionData!.AccessToken;
        else if (SessionData is null)
            return null;

        await _semaphore.WaitAsync();
        try
        {
            if (TokenIsNotExpired()) return SessionData.AccessToken;
            Result<UserAuthorizeResponse> result = await PostAsync<UpdateTokenRequest, UserAuthorizeResponse>(
                "/UpdateToken", 
                new UpdateTokenRequest() { 
                    Token = SessionData!.RefreshToken 
                });
            if (result.Success)
                SetTokens(result.Body!);
            return SessionData.AccessToken;
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

    public void SetTokens(UserAuthorizeResponse response)
    {
        SessionData = response;

        JwtSecurityTokenHandler jwt = new();
        JwtSecurityToken token = jwt.ReadJwtToken(SessionData.AccessToken);
        ValidTo = token.ValidTo;
    }

    protected bool TokenIsNotExpired()
    {
        if (SessionData is null)
            return false;

        return ValidTo >= DateTime.UtcNow.AddSeconds(15);
    }
}