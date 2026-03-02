using CommonObjects.Requests;
using CommonObjects.Responses;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IEntryApiService
{
    public Task<Result<UserAuthorizeResponse>> Enter(UserAuthorizeRequest request);
    public Task<Result<UserAuthorizeResponse>> Registration(RegistrationRequest request);
    public Task<Result> Delete();
    public Task<Result<UserAuthorizeResponse>> UpdateToken(UpdateTokenRequest request);
}
