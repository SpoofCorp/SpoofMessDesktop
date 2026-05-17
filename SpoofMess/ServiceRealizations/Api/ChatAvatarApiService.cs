using AdditionalHelpers.Services;
using CommonObjects.Requests.Avatars;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

internal class ChatAvatarApiService(
    HttpClient client,
    ISerializer serializer) : ApiService(
        client,
        serializer), IChatAvatarApiService
{
    protected override string BaseUrl => "https://localhost:7082/api/v2/ChatAvatar";

    public async Task<Result> Set(SetChatAvatarRequest request, CancellationToken token = default)
    {
        return await PostAsync($"/Set", request, token);
    }
    public async Task<Result<AvatarResponse>> Get(byte[] accessToken, CancellationToken token = default)
    {
        return await PostAsync<byte[], AvatarResponse>("/Get", accessToken, token);
    }

}
