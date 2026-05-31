using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpoofMess.ViewModels.Notifications;

public partial class NotificationInfoViewModel : ObservableObject
{
    [ObservableProperty]
    public partial Visibility Visibility { get; set; }
    public ObservableCollection<MessageModel> Notifications { get; set; } = [];

    [RelayCommand]
    public void HideAll()
    {
        Notifications.Clear();
        Visibility = Notifications.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    [RelayCommand]
    public void Hide(MessageModel message)
    {
        Notifications.Remove(message);
        Visibility = Notifications.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    public void Add(MessageModel message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Notifications.Add(message);
        });
        Visibility = Notifications.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }
}
