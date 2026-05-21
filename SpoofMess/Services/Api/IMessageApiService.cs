using CommonObjects.DTO;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IMessageApiService
{
    public Task<Result<List<MessageDTO>>> GetSkippedMessages(
        DateTime date, 
        int take = 50,
        CancellationToken token = default
        );
    
    public Task<Result<List<MessageDTO>>> GetBeforeMessages(
        Guid chatId,
        DateTime date, 
        int take = 50,
        CancellationToken token = default
        );

    public Task<Result> Delete(
        Guid messageId,
        Guid chatId, 
        CancellationToken token = default);
}
