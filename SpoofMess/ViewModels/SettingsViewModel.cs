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
    INavigationService navigationService) : AdditionalViewModel
{
    [NotifyPropertyChangedFor(nameof(MainViewVisibility))]
    [ObservableProperty]
    private AdditionalViewModel? _additionalViewModel;
    public UserInfo UserInfo { get; set; } = userInfo;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IUserAvatarService _userAvatarService = userAvatarService;
    public Visibility MainViewVisibility => AdditionalViewModel is null ? Visibility.Visible : Visibility.Collapsed;

    [RelayCommand]
    private void Profile()
    {
        AdditionalViewModel = _navigationService.GetProfileViewModel(this, Close);
    }

    [RelayCommand]
    private void Notifications()
    {

    }

    [RelayCommand]
    private void Privacy()
    {

    }

    [RelayCommand]
    private void Language()
    {

    }

    [RelayCommand]
    private void Advanced()
    {
        AdditionalViewModel = _navigationService.GetAdvancedViewModel(this, Close);
    }

    [RelayCommand]
    private async Task SetAvatar()
    {
        await _userAvatarService.Set();
    }

    private void Close()
    {
        AdditionalViewModel = null;
    }

    public override void OnClose()
    {
        AdditionalViewModel?.OnClose();
        AdditionalViewModel = null;
    }
}
