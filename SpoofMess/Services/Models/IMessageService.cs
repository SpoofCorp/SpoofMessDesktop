using CommonObjects.DTO;
using CommonObjects.Responses;
using SpoofMess.Models;

namespace SpoofMess.Services.Models;

public interface IMessageService
{
    public Task LoadSkippedMesssages(DateTime after);
    public void UploadMessage(MessageDTO message);
    public void EditHandle(EditMessageResponse message);
    public void DeleteLocal(MessageModel message);
    public Task StopEdit(MessageModel message, Chat? chat);
    public void StartEdit(MessageModel message);
    public Task SendMessage(Chat? chat, CancellationToken token = default);
}
