using CommonObjects.DTO;
using SpoofMess.Models;

namespace SpoofMess.Services.Models;

public interface IChatUserService
{
    public Task SyncChats(DateTime after, CancellationToken cancellationToken = default);
    public Task<bool> Join(Chat chat, CancellationToken cancellationToken = default);
    public void ChatCreated(ChatUserDTO chatUserDTO);
    public Task UploadManyMembers(Chat chat, CancellationToken cancellationToken = default);
}
