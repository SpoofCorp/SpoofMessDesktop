using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.Settings;

public partial class AdvancedViewModel(UserInfo userInfo, IAuthService authService) : AdditionalViewModel
{
    private readonly IAuthService _authService = authService;
    public UserInfo UserInfo { get; set; } = userInfo;

    public override async void OnClose()
    {
        await _authService.Save();
        base.OnClose();
    }
}
