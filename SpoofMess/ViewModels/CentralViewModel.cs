using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Models.API;
using SpoofMess.Services;

namespace SpoofMess.ViewModels;

public partial class CentralViewModel(
    UserInfo userInfo,
    INavigationService navigation,
    IAppLifeCycleService appLifeCycleService) : BaseViewModel(appLifeCycleService)
{
    private readonly INavigationService _navigation = navigation;
    [ObservableProperty]
    public partial ObservableObject? View { get; set; }
    [ObservableProperty]
    public partial Notification? Notification { get; set; }
    public UserInfo UserInfo { get; set; } = userInfo;


    [RelayCommand]
    private void Open()
    {
        _navigation.OpenWindow();
    }

    [RelayCommand]
    private void Close()
    {
        _navigation.CloseWindow();
    }

    [RelayCommand]
    private void Hide()
    {
        _navigation.HideWindow();
    }

    [RelayCommand]
    private void Resize()
    {
        _navigation.ResizeWindow();
    }
}