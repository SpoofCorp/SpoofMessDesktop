using CommonObjects.Requests.Avatars;
using CommonObjects.Responses;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IUserAvatarApiService
{
    public Task<Result> Set(SesUserAvatarRequest request, CancellationToken token = default);
    public Task<Result<AvatarResponse>> Get(byte[] accessToken, CancellationToken token = default);
}
