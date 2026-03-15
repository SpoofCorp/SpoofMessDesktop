using SpoofMess.Models;

namespace SpoofMess.Services.Models;

public interface IUserService
{
    public Task<User?> Get(string login);
}
