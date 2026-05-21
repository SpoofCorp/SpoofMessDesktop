using CommonObjects.DTO;
using CommonObjects.Requests.Changes;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IUserApiService
{
    public Task<Result<UserDTO>> GetByLogin(string login);

    public Task<Result> ChangeSettings(ChangeUserSettingsRequest request);
}
