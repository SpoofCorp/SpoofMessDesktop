using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.Settings;

public partial class AdvancedViewModel(UserInfo userInfo, IAuthService authService, IAppLifeCycleService appLifeCycleService) : AdditionalViewModel(appLifeCycleService)
{
    private readonly IAuthService _authService = authService;
    public UserInfo UserInfo { get; set; } = userInfo;

    public override async void OnClose()
    {
        await _authService.Save(_appLifeCycleService.AppCancellationToken);
        base.OnClose();
    }
}
