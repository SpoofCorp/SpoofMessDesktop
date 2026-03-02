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
    protected override string BaseUrl => "https://localhost:7146/api/Message";

    public async Task<Result<List<MessageDTO>>> GetSkippedMessages(DateTime date, int take = 50, CancellationToken? token = null)
    {
        try
        {
            return await GetAsync<List<MessageDTO>>($"/get-skiped?after={date}&take={take}", token);
        }
        catch(Exception ex)
        {
            return Result<List<MessageDTO>>.ErrorResult("");
        }
    }
}
