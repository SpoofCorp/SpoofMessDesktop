using CommonObjects.DTO;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IUserApiService
{
    public Task<Result<UserDTO>> GetByLogin(string login);
}
