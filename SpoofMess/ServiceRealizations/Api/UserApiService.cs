using AdditionalHelpers.Services;
using CommonObjects.DTO;
using CommonObjects.Requests.Changes;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

internal class UserApiService(
    HttpClient client,
    ISerializer serializer) : ApiService(
        client, 
        serializer), IUserApiService
{
    public async Task<Result> ChangeSettings(ChangeUserSettingsRequest request, CancellationToken cancellationToken = default) =>
        await PatchAsync(
            "api/v2/User/ChangeSettings",
            request,
            cancellationToken
        );

    public async Task<Result<UserDTO>> GetByLogin(string login, CancellationToken cancellationToken = default) =>
        await GetAsync<UserDTO>(
            $"api/v2/User/info?login={login}",
            cancellationToken
        );
}