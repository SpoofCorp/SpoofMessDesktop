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
    public async Task<Result> ChangeSettings(ChangeUserSettingsRequest request)
    {
        return await PatchAsync("api/v2/User/ChangeSettings", request);
    }

    public async Task<Result<UserDTO>> GetByLogin(string login)
    {
        return await GetAsync<UserDTO>($"api/v2/User/info?login={login}");
    }
}