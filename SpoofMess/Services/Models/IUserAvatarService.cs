using CommonObjects.Responses;
using CommonObjects.Results;

namespace SpoofMess.Services.Models;

public interface IUserAvatarService
{
    public Task<Result<AvatarResponse>> GetToken(byte[] accessToken, CancellationToken cancellationToken = default);
    public Task<Result> Set();
}