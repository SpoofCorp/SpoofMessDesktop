using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models;

namespace SpoofMess.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    public UserInfo UserInfo { get; set; }
}
