using CommonObjects.DTO;
using CommonObjects.Requests.Changes;
using CommonObjects.Requests.Messages;
using CommonObjects.Responses;
using SpoofMess.Models;

namespace SpoofMess.Services.Api;

public interface INotificationApiService
{
    public event Action<MessageDTO> OnMessageReceived;

    public event Action<EditMessageResponse> OnMessageEdited;

    public event Action<UpdateUserInfo> OnUserUpdated;

    public event Action<ChangeChatSettingsRequest> OnChatUpdated;

    public Task SendMessage(CreateMessageRequest message);

    public Task EditMessage(EditMessageRequest message);

    public Task<bool> DeleteMessage(MessageModel message);
}
