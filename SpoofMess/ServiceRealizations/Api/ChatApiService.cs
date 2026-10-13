using AdditionalHelpers.Services;
using CommonObjects.DTO;
using CommonObjects.Requests;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

class ChatApiService(
    HttpClient client,
    ISerializer serializer
    ) : ApiService(client, serializer), IChatApiService
{
    public async Task<Result<ChatDTO>> GetChat(Guid id)
    {
        Result<ChatDTO> result = await GetAsync<ChatDTO>($"api/v2/Chat/get?chatId={id}");
        return result;
    }

    public async Task<Result<ChatDTO>> Create(CreateChatRequest chat, CancellationToken token)
    {
        Result<ChatDTO> chatResult = await PostAsync<CreateChatRequest, ChatDTO>("api/v2/Chat/CreateChat", chat, token);

        return chatResult;
    }
}
