using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models;

namespace SpoofMess.ViewModels;

public partial class ProfileViewModel(UserInfo userInfo) : ObservableObject
{
    public UserInfo UserInfo { get; set; } = userInfo;
}
