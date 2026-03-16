using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;

namespace SpoofMess.ViewModels;

public partial class SettingsViewModel(UserInfo userInfo) : ObservableObject
{
    public UserInfo UserInfo { get; set; } = userInfo;

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
