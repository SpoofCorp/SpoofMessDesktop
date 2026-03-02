using SpoofMess.Models;

namespace SpoofMess.Services;

public interface INotificationService
{

    [Obsolete("Now it's a stub. You can use it, but result will be uncorrect.")]
    public void ShowToast(Notification notification);
    public void ShowMessageBox(Notification notification);
}
