using SpoofMess.Models;

namespace SpoofMess.ServiceRealizations;

public class EventHandler
{
    public delegate void Delete(MessageModel message);
    public delegate void Edit(MessageModel message);
    public delegate void Add(MessageModel message);
    public delegate void AddChat(Chat chat);
    public delegate void UserAvatarUpdated(FileObject avatar, User user);
    public delegate void ChatAvatarUpdated(FileObject avatar, Chat chat);

    public static event Delete OnDelete = null!;
    public static event Edit OnEdit = null!;
    public static event Add OnAdd = null!;
    public static event AddChat OnAddChatAdded = null!;
    public static event UserAvatarUpdated OnUserAvatarUpdated = null!;
    public static event ChatAvatarUpdated OnChatAvatarUpdated = null!;

    public static void NotifyAddChat(Chat chat) =>
        OnAddChatAdded?.Invoke(chat);
    public static void NotifyDelete(MessageModel message) =>
        OnDelete?.Invoke(message);
    public static void NotifyEdit(MessageModel message) =>
        OnEdit?.Invoke(message);
    public static void NotifyAdd(MessageModel message) =>
        OnAdd?.Invoke(message);
    public static void NotifyUserAvatarUpdated(FileObject avatar, User user) =>
        OnUserAvatarUpdated?.Invoke(avatar, user);
    public static void NotifyChatAvatarUpdated(FileObject avatar, Chat chat) =>
        OnChatAvatarUpdated?.Invoke(avatar, chat);
}
