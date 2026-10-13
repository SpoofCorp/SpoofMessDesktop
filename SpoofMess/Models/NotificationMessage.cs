using System.Collections.ObjectModel;

namespace SpoofMess.Models;

public class NotificationMessage
{
    public MessageModel Message { get; set; }

    public NotificationMessage(MessageModel message, ObservableCollection<NotificationMessage> collection)
    {
        Message = message;
        Delete(collection);
    }

    public async void Delete(ObservableCollection<NotificationMessage> collection)
    {
        await Task.Delay(5000);
        collection.Remove(this);
    }
}
