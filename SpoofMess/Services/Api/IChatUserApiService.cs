using CommonObjects.DTO;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IChatUserApiService
{
    public Task<Result<List<ChatUserDTO>>> GetChats(DateTime after);
}
