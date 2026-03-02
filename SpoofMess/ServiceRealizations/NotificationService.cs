using SpoofMess.Models;
using SpoofMess.Services;
using System.Windows;

namespace SpoofMess.ServiceRealizations;

public class NotificationService : INotificationService
{
    public void ShowMessageBox(Notification notification)
    {
        MessageBox.Show(
            notification.Text, 
            notification.Type.ToString(),
            MessageBoxButton.OK
            );
    }

    public void ShowToast(Notification notification)
    {
        ShowMessageBox(notification);
    }
}
