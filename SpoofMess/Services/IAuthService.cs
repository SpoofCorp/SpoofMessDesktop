using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Models.UpdateModels;

namespace SpoofMess.Services;

public interface IAuthService
{
    public bool IsAuthorized { get; set; }
    public Task<Result> Initialize();
    public Task<string?> GetAccess();
    public Task SetTokens(UserAuthorizeResponse response);
    public Task Save();
    public Task Update(UserInfoUpdateModel changes);
}
