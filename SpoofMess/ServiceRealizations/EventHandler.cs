using SpoofMess.Models;

namespace SpoofMess.ServiceRealizations;

public class EventHandler
{
    public delegate void Delete(MessageModel message);
    public delegate void Edit(MessageModel message);
    public delegate void Add(MessageModel message);

    public static event Delete OnDelete = null!;
    public static event Edit OnEdit = null!;
    public static event Add OnAdd = null!;

    public static void NotifyDelete(MessageModel message) =>
        OnDelete.Invoke(message);
    public static void NotifyEdit(MessageModel message) =>
        OnEdit.Invoke(message);
    public static void NotifyAdd(MessageModel message) =>
        OnAdd.Invoke(message);
}
