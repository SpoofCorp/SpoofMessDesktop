using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.Settings;

public partial class AdvancedViewModel(
    AppSessionSettings appSessionSettings,
    IAuthService authService,
    IAppLifeCycleService appLifeCycleService) : AdditionalViewModel(appLifeCycleService)
{
    private readonly IAuthService _authService = authService;
    public AppSessionSettings AppSessionSettings { get; set; } = new();

    protected override Task InitializeAsync(bool initialize)
    {
        AppSessionSettings.Init(appSessionSettings);
        return base.InitializeAsync(initialize);
    }

    public override async void OnClose()
    {
        await _authService.SaveAppSessionSettings(_appLifeCycleService.AppCancellationToken);
        base.OnClose();
    }
}
