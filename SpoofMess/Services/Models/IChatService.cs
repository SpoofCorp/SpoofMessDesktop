using CommonObjects.DTO;
using CommonObjects.Requests.Changes;
using CommonObjects.Responses;
using SpoofMess.Models;

namespace SpoofMess.Services.Models;

public interface IChatService
{
    public Task<Chat?> Get(Guid id, bool saveToLocal = true);
    public Task AddChats(List<ChatUserDTO> chats);
    public Task CreateChat(Chat chat);
    public void Update(ChangeChatSettingsRequest request);
    public void OnMessageAdded(MessageModel message); 
    public void OnChatAvatarUpdated(ChatAvatarResponse response);

    public event Action<Chat, int?> ChatAdded;

    public event Action<int, int> ChatMoved;
}
