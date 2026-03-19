using CommonObjects.DTO;
using CommonObjects.Requests;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IChatApiService
{
    public Task<Result<ChatDTO>> GetChat(Guid id);

    public Task<Result<ChatDTO>> Create(CreateChatRequest chat, CancellationToken token = default);
}
