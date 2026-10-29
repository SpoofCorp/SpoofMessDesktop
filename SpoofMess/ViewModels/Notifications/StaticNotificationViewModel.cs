using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Services;
using System.Windows;

namespace SpoofMess.ViewModels.Notifications;

public partial class StaticNotificationViewModel(MusicNotification musicNotification, IAppLifeCycleService appLifeCycleService) : BaseViewModel(appLifeCycleService)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Visibility))]
    public partial ObservableObject? ViewModel { get; set; }
    public List<ObservableObject> ViewModels { get; set; } = [musicNotification];

    public Visibility Visibility => ViewModel is null ? Visibility.Collapsed : Visibility.Visible;

    protected override async Task InitializeAsync(bool initialize)
    {
        await musicNotification.InitializeAsync();
        ServiceRealizations.EventHandler.OnChangeStaticNotificationViewModel += ChangeStaticNotificationViewModel;
        await base.InitializeAsync(initialize);
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        foreach (ObservableObject observableObject in ViewModels)
            if (observableObject is BaseViewModel baseViewModel)
                await baseViewModel.DisposeAsync();
        ServiceRealizations.EventHandler.OnChangeStaticNotificationViewModel -= ChangeStaticNotificationViewModel;
        await base.DisposeAsync(disposing);
    }

    [RelayCommand]
    private void Close()
    {
        ViewModel = null;
        ServiceRealizations.EventHandler.NotifyChangeStaticNotificationViewModel(null);
    }

    public void ChangeStaticNotificationViewModel(BaseViewModel? baseViewModel)
    {
        if (ViewModel == baseViewModel || baseViewModel == null)
            return;
        ViewModel = baseViewModel;
    }
}
