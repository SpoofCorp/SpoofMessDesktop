using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Models.UpdateModels;

namespace SpoofMess.Services;

public interface IAuthService
{
    public bool IsAuthorized { get; set; }
    public Task<Result> Initialize(CancellationToken cancellationToken = default);
    public Task<string?> GetAccess(CancellationToken cancellationToken = default);
    public Task SetTokens(UserAuthorizeResponse response, CancellationToken cancellationToken = default);
    public Task Save(CancellationToken cancellationToken = default);
    public Task Update(UserInfoUpdateModel changes, CancellationToken cancellationToken = default);
}
