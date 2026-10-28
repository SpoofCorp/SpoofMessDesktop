using CommonObjects.DTO;
using CommonObjects.Requests.Changes;
using CommonObjects.Requests.Messages;
using CommonObjects.Responses;
using SpoofMess.Bases;
using SpoofMess.Models;

namespace SpoofMess.Services.Api;

public interface INotificationApiService : IAsyncInitializable
{
    public event Action<MessageDTO> OnMessageReceived;

    public event Action<Guid, Guid> OnMessageDeleted;

    public event Action<EditMessageResponse> OnMessageEdited;

    public event Action<UpdateUserInfo> OnUserUpdated;

    public event Action<ChangeChatSettingsRequest> OnChatUpdated;

    public event Action<ChatAvatarResponse> OnChatAvatarUpdated;

    public event Action<ChatUserDTO> OnChatCreated;

    public Task SendMessage(CreateMessageRequest message, CancellationToken cancellationToken = default);

    public Task EditMessage(EditMessageRequest message, CancellationToken cancellationToken = default);

    public Task<bool> DeleteMessage(MessageModel message, CancellationToken cancellationToken = default);
}
