using CommonObjects.DTO;
using CommonObjects.Responses;
using SpoofMess.Models;

namespace SpoofMess.Services.Models;

public interface IMessageService
{
    public Task LoadSkippedMesssages(DateTime after, CancellationToken token = default);
    public void UploadMessage(MessageDTO message);
    public void EditHandle(EditMessageResponse message);
    public void DeleteLocal(MessageModel message);
    public Task StopEdit(Chat? chat, CancellationToken token = default);
    public void StartEdit(MessageModel message);
    public void OnDelete(Guid messageId, Guid chatId);
    public Task SendMessage(Chat? chat, CancellationToken token = default);
    public Task UploadMessagesAfterDate(Chat chat, CancellationToken token = default);
}
