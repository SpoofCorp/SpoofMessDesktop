using CommonObjects.DTO;
using SpoofMess.Models;
using System.Collections.ObjectModel;

namespace SpoofMess.Services.Models;

public interface IChatService
{
    public ObservableCollection<Chat> Chats { get; set; }
    public Task<Chat?> Get(Guid id);
    public void AddChats(List<ChatUserDTO> chats);
}
