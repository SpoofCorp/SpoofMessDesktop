using CommonObjects.Results;
using SpoofMess.Models;

namespace SpoofMess.Services.Models;

public interface IChatAvatarService
{
    public Task<Result> Set(Chat chat, CancellationToken cancellationToken = default);

    public Task UploadAvatar(Chat chat, CancellationToken cancellationToken = default);
    public Task OnChatAvatarUpdated(FileObject avatar, CancellationToken cancellationToken = default);
}