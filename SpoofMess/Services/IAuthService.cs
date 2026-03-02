using CommonObjects.Responses;

namespace SpoofMess.Services;

public interface IAuthService
{
    public Task<string?> GetAccess();
    public void SetTokens(UserAuthorizeResponse response);
}
