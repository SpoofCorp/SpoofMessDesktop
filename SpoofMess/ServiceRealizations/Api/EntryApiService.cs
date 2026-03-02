using AdditionalHelpers.Services;
using CommonObjects.Requests;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

public class EntryApiService(
    HttpClient client,
    ISerializer serializer
    ) : ApiService(client, serializer), IEntryApiService
{
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
        return result;
    }

    public Task<Result<UserAuthorizeResponse>> Registration(RegistrationRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserAuthorizeResponse>> UpdateToken(UpdateTokenRequest request)
    {
        throw new NotImplementedException();
    }
}
