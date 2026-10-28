using CommonObjects.DTO;
using CommonObjects.Requests.Members;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IChatUserApiService
{
    public Task<Result<List<ChatUserDTO>>> GetChats(DateTime after, CancellationToken cancellationToken = default);
    public Task<Result> Join(JoinToChatRequest request, CancellationToken cancellationToken = default);
    public Task<Result<List<ChatMemberDTO>>> GetMembersBefore(
        Guid chatId,
        string lastLogin,
        DateTime? before,
        CancellationToken cancellationToken = default);
}
