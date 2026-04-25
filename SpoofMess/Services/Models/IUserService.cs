using CommonObjects.DTO;
using SpoofMess.Models;

namespace SpoofMess.Services.Models;

public interface IUserService
{
    public Task<User?> Get(string login, byte[]? avatarId, byte[]? avatarToken, string originalAvatarName);
    public void OnUserUpdated(UpdateUserInfo updateUser);
}
