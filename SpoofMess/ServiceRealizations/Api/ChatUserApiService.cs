using AdditionalHelpers.Services;
using CommonObjects.DTO;
using CommonObjects.Requests.Members;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

public class ChatUserApiService(
    HttpClient client,
    ISerializer serializer
    ) : ApiService(client, serializer), IChatUserApiService
{

    public Task<ChatDTO> GetChat(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<ChatUserDTO>>> GetChats(DateTime after)
    {
        Result<List<ChatUserDTO>> result = await GetAsync<List<ChatUserDTO>>($"api/ChatUser/get-chats?after={after:yyyy-MM-ddTHH:mm:ssZ}");
        if (result.Success)
            return result;
        //Need show exception
        return result;
    }

    public async Task<Result> Join(JoinToChatRequest request, CancellationToken cancellationToken = default) =>
        await PostAsync("api/ChatUser/join", request, cancellationToken);

    public async Task<Result<List<ChatMemberDTO>>> GetMembersBefore(
        Guid chatId,
        string lastLogin,
        DateTime? before,
        CancellationToken cancellationToken = default) =>
        await GetAsync<List<ChatMemberDTO>>($"api/ChatUser/getChatMembers?chatId={chatId}&lastLogin={lastLogin}&before={before}", cancellationToken);
}
