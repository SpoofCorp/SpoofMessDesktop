using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Models;
using System.Windows;

namespace SpoofMess.ViewModels;

public partial class SettingsViewModel(
    UserInfo userInfo,
    IUserAvatarService userAvatarService,
    INavigationService navigationService,
    IAppLifeCycleService appLifeCycleService) : PlacementAdditionalViewModel(appLifeCycleService)
{
    public UserInfo UserInfo { get; set; } = userInfo;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IUserAvatarService _userAvatarService = userAvatarService;
    [ObservableProperty]
    public partial Visibility MainViewVisibility { get; set; }

    [RelayCommand]
    private void Profile()
    {
        _navigationService.SetProfileViewModel(this);
    }

    [RelayCommand]
    private void Notifications()
    {
        _navigationService.SetSoundsAndNotificationsViewModel(this);
    }

    [RelayCommand]
    private void Privacy()
    {

    }

    [RelayCommand]
    private void Language()
    {
        _navigationService.SetLanguageViewModel(this);
    }

    [RelayCommand]
    private void Advanced()
    {
        _navigationService.SetAdvancedViewModel(this);
    }

    [RelayCommand]
    private void Design()
    {
        _navigationService.SetDesignViewModel(this);
    }

    [RelayCommand]
    private async Task SetAvatar()
    {
        await _userAvatarService.Set(_appLifeCycleService.AppCancellationToken);
    }

    public override void OnClose()
    {
        AdditionalViewModel?.OnClose();
        AdditionalViewModel = null;
    }

    public override void ShowAdditionalViewModel(AdditionalViewModel additionalViewModel)
    {
        MainViewVisibility = Visibility.Collapsed;
        base.ShowAdditionalViewModel(additionalViewModel);
    }

    protected override void CloseAdditionalViewModel()
    {
        MainViewVisibility = Visibility.Visible;
        base.CloseAdditionalViewModel();
    }
}
