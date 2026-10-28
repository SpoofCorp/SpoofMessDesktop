using CommonObjects.Requests;
using CommonObjects.Responses;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IEntryApiService
{
    public Task<Result<UserAuthorizeResponse>> Enter(UserAuthorizeRequest request, CancellationToken cancellationToken = default);
    public Task<Result<UserAuthorizeResponse>> Registration(RegistrationRequest request, CancellationToken cancellationToken = default);
    public Task<Result> Delete(CancellationToken cancellationToken = default);
    public Task<Result<UserAuthorizeResponse>> UpdateToken(UpdateTokenRequest request, CancellationToken cancellationToken = default);
}
