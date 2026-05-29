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
    [ObservableProperty]
    private AdditionalViewModel? _additionalViewModel;
    public UserInfo UserInfo { get; set; } = userInfo;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IUserAvatarService _userAvatarService = userAvatarService;
    [ObservableProperty]
    public partial Visibility MainViewVisibility { get; set; }

    [RelayCommand]
    private void Profile()
    {
        SetAdditionalViewModel(_navigationService.GetProfileViewModel(this, CloseAdditional));
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
        SetAdditionalViewModel(_navigationService.GetLanguageViewModel(this, CloseAdditional));
    }

    [RelayCommand]
    private void Advanced()
    {
        SetAdditionalViewModel(_navigationService.GetAdvancedViewModel(this, CloseAdditional));
    }

    [RelayCommand]
    private void Design()
    {
        SetAdditionalViewModel(_navigationService.GetDesignViewModel(this, CloseAdditional));
    }

    [RelayCommand]
    private async Task SetAvatar()
    {
        await _userAvatarService.Set();
    }

    private async void CloseAdditional()
    {
        MainViewVisibility = Visibility.Visible;
        AdditionalViewModel = null;
    }

    private void SetAdditionalViewModel(AdditionalViewModel viewModel)
    {
        AdditionalViewModel = viewModel;
        MainViewVisibility = Visibility.Collapsed;

    }

    public override void OnClose()
    {
        AdditionalViewModel?.OnClose();
        AdditionalViewModel = null;
    }
}
