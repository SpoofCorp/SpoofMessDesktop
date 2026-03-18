using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ViewModels;

public partial class SettingsViewModel(UserInfo userInfo, INavigationService navigationService) : ObservableObject
{
    public UserInfo UserInfo { get; set; } = userInfo;
    private readonly INavigationService _navigationService = navigationService;

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
}
