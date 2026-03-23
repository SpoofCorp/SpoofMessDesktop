using CommonObjects.DTO;
using CommonObjects.Requests.Messages;

namespace SpoofMess.Services.Api;

public interface INotificationApiService
{
    public event Action<MessageDTO> OnMessageReceived;
    public Task SendMessage(CreateMessageRequest message);
}
