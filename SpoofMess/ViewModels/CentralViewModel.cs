using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Models.API;
using SpoofMess.Services;

namespace SpoofMess.ViewModels;

public partial class CentralViewModel(UserInfo userInfo, INavigationService navigation) : ObservableObject
{
    private readonly INavigationService _navigation = navigation;
    [ObservableProperty]
    private ObservableObject? _view;
    [ObservableProperty]
    private Notification? _notification;

    public UserInfo UserInfo { get; set; } = userInfo;


    [RelayCommand]
    private void Open()
    {
        _navigation.OpenWindow();
    }
}