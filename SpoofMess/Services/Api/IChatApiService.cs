using CommonObjects.DTO;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IChatApiService
{
    public Task<Result<ChatDTO>> GetChat(Guid id);
}
