using SpoofMess.Models.API;

namespace SpoofMess.Services;

public interface INotificationService
{
    public void ShowError(string? error);

    public void ShowToast(Notification notification);

    public void ShowMessageBox(Notification notification);
}
