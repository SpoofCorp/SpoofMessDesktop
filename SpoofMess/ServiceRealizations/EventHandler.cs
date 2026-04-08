using SpoofMess.Models;

namespace SpoofMess.ServiceRealizations;

public class EventHandler
{
    public delegate void Delete(MessageModel message);
    public delegate void Edit(MessageModel message);

    public static Delete OnDelete = null!;
    public static Edit OnEdit = null!;
}
