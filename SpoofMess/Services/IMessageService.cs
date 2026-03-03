using CommonObjects.Requests.Messages;
using SpoofMess.Models;

namespace SpoofMess.Services;

public interface IMessageService
{
    public event Action<MessageModel> OnMessageReceived;
    public Task SendMessage(CreateMessageRequest message);
}
