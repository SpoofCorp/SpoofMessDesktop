using CommonObjects.Results;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Models;

namespace SpoofMess.ViewModels;

public partial class SettingsViewModel(
    UserInfo userInfo, 
    IUserAvatarService userAvatarService,
    INavigationService navigationService) : ObservableObject
{
    public UserInfo UserInfo { get; set; } = userInfo;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IUserAvatarService _userAvatarService = userAvatarService;

    [RelayCommand]
    private void Profile()
    {

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
    private async Task SetAvatar()
    {
        Result result = await _userAvatarService.Set();
    }
}
