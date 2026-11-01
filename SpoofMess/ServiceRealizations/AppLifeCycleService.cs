using Microsoft.Extensions.DependencyInjection;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Models;

namespace SpoofMess.ServiceRealizations;

public class AppLifeCycleService(
    IAuthService authService,
    UserInfo userInfo,
    IServiceProvider serviceProvider
) : IAppLifeCycleService, IDisposable
{
    protected UserInfo _userInfo = userInfo;
    protected CancellationTokenSource _cts = new();
    private readonly IAuthService _authService = authService;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private INavigationService _navigationService = null!;

    public async Task OnExitAsync()
    {
        if(_authService.IsAuthorized)
            await _authService.Save();
        await _cts.CancelAsync();
        await _navigationService.CloseWindow();
    }

    public async Task OnCloseAsync()
    {
        if (_userInfo.HideToTray)
        {
            _navigationService.HideToTrayWindow();
        }
        else
        {
            await OnExitAsync();
        }
    }

    public async Task OnStartupAsync()
    {
        IUserService userService = _serviceProvider.GetRequiredService<IUserService>();
        INotificationService notificationService = _serviceProvider.GetRequiredService<INotificationService>();
        _navigationService = _serviceProvider.GetRequiredService<INavigationService>();
        notificationService.Initialize();
        await _navigationService.InitializeAsync();
        if (await _authService.Initialize())
        {
            await userService.SyncAccount();
            _navigationService!.ShowCentralViewWithMain();
        }
        else
            _navigationService!.ShowCentralViewWithAuthorization();
    }

    public CancellationToken AppCancellationToken =>
        _cts.Token;

    public void Dispose()
    {
        _cts.Cancel();
        GC.SuppressFinalize(this);
    }
}
