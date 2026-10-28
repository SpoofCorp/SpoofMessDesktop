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

    public Task<Result> Delete(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<UserAuthorizeResponse>> Enter(UserAuthorizeRequest request, CancellationToken cancellationToken = default)
    {
        Result<UserAuthorizeResponse> result = await PostAsync<UserAuthorizeRequest, UserAuthorizeResponse>(
            "api/v1/Entrance/Enter",
            request,
            cancellationToken
        );
        if(result.Success)
            await _authService.SetTokens(
                result.Body!,
                cancellationToken
            );
        return result;
    }

    public async Task<Result<UserAuthorizeResponse>> Registration(RegistrationRequest request, CancellationToken cancellationToken = default)
    {
        Result<UserAuthorizeResponse> result = await PostAsync<RegistrationRequest, UserAuthorizeResponse>(
            "api/v1/Entrance/Registration",
            request,
            cancellationToken
        );
        if (result.Success)
            await _authService.SetTokens(
                result.Body!,
                cancellationToken
            );
        return result;
    }

    public Task<Result<UserAuthorizeResponse>> UpdateToken(UpdateTokenRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
