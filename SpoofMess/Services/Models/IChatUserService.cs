using CommonObjects.DTO;
using SpoofMess.Models;

namespace SpoofMess.Services.Models;

public interface IChatUserService
{
    public Task SyncChats(DateTime after);
    public Task<bool> Join(Chat chat);
    public void ChatCreated(ChatUserDTO chatUserDTO);
}
