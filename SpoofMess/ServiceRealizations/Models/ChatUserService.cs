using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;

namespace SpoofMess.ServiceRealizations.Models;

public class ChatUserService(
    IChatUserApiService chatUserApiService,
    IChatService chatService) : IChatUserService
{
    private readonly IChatUserApiService _chatUserApiService = chatUserApiService;
    private readonly IChatService _chatService = chatService;

    public async Task SyncChats(DateTime after)
    {
        Result<List<ChatUserDTO>> chats = await _chatUserApiService.GetChats(after);
        if (chats.Success)
            _chatService.AddChats(chats.Body!);
    }
}
