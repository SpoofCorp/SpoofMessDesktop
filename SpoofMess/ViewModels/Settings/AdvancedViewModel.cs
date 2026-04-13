using SpoofMess.Models;

namespace SpoofMess.ViewModels.Settings;

public partial class AdvancedViewModel(UserInfo userInfo) : AdditionalViewModel
{
    public UserInfo UserInfo { get; set; } = userInfo;
}
