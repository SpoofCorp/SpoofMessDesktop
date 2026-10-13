using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpoofMess.ViewModels.Notifications;

public partial class NotificationInfoViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    public partial Visibility Visibility { get; set; }
    public ObservableCollection<NotificationMessage> Notifications { get; set; } = [];

    public NotificationInfoViewModel()
    {
        Notifications.CollectionChanged += Notifications_CollectionChanged;
    }

    [RelayCommand]
    public void HideAll()
    {
        Notifications.Clear();
    }

    [RelayCommand]
    public void Hide(NotificationMessage message)
    {
        Notifications.Remove(message);
    }

    public void Add(MessageModel message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Notifications.Add(new(message, Notifications));
        });
    }

    private void Notifications_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Visibility = Notifications.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    public void Dispose()
    {
        Notifications.CollectionChanged -= Notifications_CollectionChanged;
        GC.SuppressFinalize(this);
    }
}
