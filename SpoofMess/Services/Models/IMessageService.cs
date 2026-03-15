using CommonObjects.DTO;

namespace SpoofMess.Services.Models;

public interface IMessageService
{
    public Task LoadSkippedMesssages(DateTime after);
    public Task UploadMessage(MessageDTO message);
}
