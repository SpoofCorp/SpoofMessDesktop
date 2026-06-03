using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Models.API;
using SpoofMess.Services;
using SpoofMess.ViewModels;
using System.Windows;

namespace SpoofMess.ServiceRealizations;

public class NotificationService(UserInfo userInfo, CentralViewModel centralViewModel, INavigationService navigationService) : INotificationService
{
    private readonly UserInfo _userInfo = userInfo;
    private readonly CentralViewModel _centralViewModel = centralViewModel;
    private readonly INavigationService _navigationService = navigationService;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public void InitializeAsync()
    {
        EventHandler.OnAdd += ShowNewMessageNotification;
    }

    public void ShowMessageBox(Notification notification)
    {
        MessageBox.Show(
            notification.Text, 
            notification.Type.ToString(),
            MessageBoxButton.OK
            );
    }

    public async void ShowNewMessageNotification(MessageModel message)
    {
        if (!_userInfo.ShowMessagesNotifications || _navigationService.IsActive)
            return;
        _navigationService.ShowNotification(message);
    }

    public async void ShowToast(Notification notification)
    {
        if (!_userInfo.ShowErrorNotifications)
            return;
        await _semaphore.WaitAsync();
        try
        {
            _centralViewModel.Notification = notification;
            await Task.Delay(notification.Time);
            _centralViewModel.Notification = null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void ShowError(string? text) =>
        ShowToast(new(text, Enums.NotificationType.Error));

    public void ShowWarning(string? text) =>
        ShowToast(new(text, Enums.NotificationType.Warning));

    public void ShowFail(string? text) =>
        ShowToast(new(text, Enums.NotificationType.Fail));

    public void ShowInfo(string? text) =>
        ShowToast(new(text, Enums.NotificationType.Info));

    public void ShowError(Result error) =>
        ShowError(error.Error ?? error.Message);

    public void ShowError<T>(Result<T> error) =>
        ShowError(error.Error ?? error.Message);
}
