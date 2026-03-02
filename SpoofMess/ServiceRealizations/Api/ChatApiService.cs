using AdditionalHelpers.Services;
using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

class ChatApiService(
    HttpClient client,
    ISerializer serializer
    ) : ApiService(client, serializer), IChatApiService
{
    protected override string BaseUrl => "https://localhost:7082/api/v2/Chat";

    public async Task<Result<ChatDTO>> GetChat(Guid id)
    {
        Result<ChatDTO> result = await GetAsync<ChatDTO>("?chatId={id}");
        if (result.Success)
            return result;
        return Result<ChatDTO>.ErrorResult("");
    }
}
