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
    public async Task<Result<ChatDTO>> GetChat(Guid id, CancellationToken cancellationToken = default) =>
        await GetAsync<ChatDTO>($"api/v2/Chat/get?chatId={id}", cancellationToken);

    public async Task<Result<ChatDTO>> Create(CreateChatRequest chat, CancellationToken cancellationToken = default) =>
        await PostAsync<CreateChatRequest, ChatDTO>("api/v2/Chat/CreateChat", chat, cancellationToken);
}
