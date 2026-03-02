using AdditionalHelpers.Services;
using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

public class ChatUserApiService(
    HttpClient client,
    ISerializer serializer
    ) : ApiService(client, serializer), IChatUserApiService
{
    protected override string BaseUrl => "https://localhost:7082/api/ChatUser";

    public Task<ChatDTO> GetChat(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<ChatUserDTO>>> GetChats(DateTime after)
    {
        try
        {
            Result<List<ChatUserDTO>> result = await GetAsync<List<ChatUserDTO>>($"/get-chats?after={after}");
            if (result.Success)
                return result;
            //Need show exception
            return result;
        }
        catch(Exception ex)
        {
            return Result<List<ChatUserDTO>>.ErrorResult("");
        }
    }
}
