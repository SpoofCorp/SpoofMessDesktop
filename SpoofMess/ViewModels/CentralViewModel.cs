using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models;
using SpoofMess.Models.API;

namespace SpoofMess.ViewModels;

public partial class CentralViewModel(UserInfo userInfo) : ObservableObject
{
    [ObservableProperty]
    private ObservableObject? _view;
    [ObservableProperty]
    private Notification? _notification;

    public UserInfo UserInfo { get; set; } = userInfo;
}
