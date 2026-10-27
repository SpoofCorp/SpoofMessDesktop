using Microsoft.Extensions.DependencyInjection;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Models;

namespace SpoofMess.ServiceRealizations;

public class AppLifeCycleService(
    IAuthService authService,
    IServiceProvider serviceProvider
) : IAppLifeCycleService, IDisposable
{
    protected CancellationTokenSource _cts = new();
    private readonly IAuthService _authService = authService;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task OnExitAsync()
    {
        if(_authService.IsAuthorized)
            await _authService.Save();
    }

    public async Task OnStartupAsync()
    {
        IUserService userService = _serviceProvider.GetRequiredService<IUserService>();
        INotificationService notificationService = _serviceProvider.GetRequiredService<INotificationService>();
        INavigationService navigationService = _serviceProvider.GetRequiredService<INavigationService>();
        notificationService.Initialize();
        await navigationService.InitializeAsync();
        if (await _authService.Initialize())
        {
            await userService.SyncAccount();
            navigationService!.ShowCentralViewWithMain();
        }
        else
            navigationService!.ShowCentralViewWithAuthorization();
    }

    public void Dispose()
    {
        _cts.Cancel();
        GC.SuppressFinalize(this);
    }
}
