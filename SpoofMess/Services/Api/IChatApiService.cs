using CommonObjects.DTO;
using CommonObjects.Requests;
using CommonObjects.Requests.Changes;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IChatApiService
{
    public Task<Result> ChangeSettings(ChangeChatSettingsRequest request, CancellationToken cancellationToken = default);

    public Task<Result<ChatDTO>> GetChat(Guid id, CancellationToken token = default);

    public Task<Result<ChatDTO>> Create(CreateChatRequest chat, CancellationToken token = default);
}
