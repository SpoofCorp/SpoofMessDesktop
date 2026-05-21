using CommonObjects.DTO;
using SpoofMess.Models;

namespace SpoofMess.Services.Models;

public interface IUserService
{
    public Task<User?> Get(string login, CancellationToken cancellationToken = default);
    public void OnUserUpdated(UpdateUserInfo updateUser);

    public void AvatarUpdateHandler(FileObject avatar, User user);
    public Task SyncAccount(CancellationToken cancellationToken = default);
}
