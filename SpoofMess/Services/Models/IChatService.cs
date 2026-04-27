using CommonObjects.DTO;
using CommonObjects.Requests.Changes;
using SpoofMess.Models;
using System.Collections.ObjectModel;

namespace SpoofMess.Services.Models;

public interface IChatService
{
    public ObservableCollection<Chat> Chats { get; set; }
    public Task<Chat?> Get(Guid id);
    public Task AddChats(List<ChatUserDTO> chats);
    public Task CreateChat(Chat chat);
    public void Update(ChangeChatSettingsRequest request);
}
