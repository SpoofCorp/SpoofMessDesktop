using CommonObjects.Requests.Avatars;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IChatAvatarApiService
{
    public Task<Result> Set(SetChatAvatarRequest request, CancellationToken token = default);
}
