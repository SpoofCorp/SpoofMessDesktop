using AdditionalHelpers.Services;
using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

public class MessageApiService(
    HttpClient client,
    ISerializer serializer
    ) : ApiService(
        client,
        serializer
        ), IMessageApiService
{
    public async Task<Result<List<MessageDTO>>> GetSkippedMessages(DateTime after, int take = 50, CancellationToken token = default)
    {
        return await GetAsync<List<MessageDTO>>($"api/Message/get-skiped?after={after:yyyy-MM-ddTHH:mm:ssZ}&take={take}", token);
    }

    public async Task<Result<List<MessageDTO>>> GetBeforeMessages(Guid chatId, DateTime after, int take = 50, CancellationToken token = default)
    {
        return await GetAsync<List<MessageDTO>>($"api/Message/get-before?chatId={chatId}&after={after:yyyy-MM-ddTHH:mm:ssZ}&take={take}", token);
    }

    public async Task<Result> Delete(Guid messageId, Guid chatId, CancellationToken token = default)
    {
        return await DeleteAsync($"api/Message/delete?messageId={messageId}&chatId={chatId}", token);
    }
}
