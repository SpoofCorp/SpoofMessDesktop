using CommonObjects.Responses;

namespace SpoofMess.Services;

public interface IAuthService
{
    public Task<bool> Initialize();
    public Task<string?> GetAccess();
    public Task SetTokens(UserAuthorizeResponse response);
    public Task Save();
}
