using AdditionalHelpers.Services;
using CommonObjects.Requests;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

public class EntryApiService(
    HttpClient client,
    ISerializer serializer,
    IAuthService authService
    ) : ApiService(client, serializer), IEntryApiService
{
    private readonly IAuthService _authService = authService;
    protected override string BaseUrl => "https://localhost:7217/api/v1/Entrance";

    public Task<Result> Delete()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<UserAuthorizeResponse>> Enter(UserAuthorizeRequest request)
    {
        Result<UserAuthorizeResponse> result = await PostAsync<UserAuthorizeRequest, UserAuthorizeResponse>(
            $"/Enter",
            request);
        if(result.Success)
            _authService.SetTokens(result.Body!);
        return result;
    }

    public async Task<Result<UserAuthorizeResponse>> Registration(RegistrationRequest request)
    {
        Result<UserAuthorizeResponse> result = await PostAsync<RegistrationRequest, UserAuthorizeResponse>(
            $"/Registration",
            request);
        if (result.Success)
            _authService.SetTokens(result.Body!);
        return result;
    }

    public Task<Result<UserAuthorizeResponse>> UpdateToken(UpdateTokenRequest request)
    {
        throw new NotImplementedException();
    }
}
