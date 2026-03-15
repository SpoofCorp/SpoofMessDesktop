using AdditionalHelpers.Services;
using CommonObjects.DTO;
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
    protected override string BaseUrl => "https://localhost:7082/api/v2/User";

    public async Task<Result<UserDTO>> GetByLogin(string login)
    {
        return await GetAsync<UserDTO>($"/info?login={login}");
    }
}
