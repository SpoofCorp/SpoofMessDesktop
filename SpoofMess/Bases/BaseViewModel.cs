using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Services;

namespace SpoofMess.Bases;

public partial class BaseViewModel(IAppLifeCycleService appLifeCycleService) : ObservableObject, IAsyncInitializable, IAsyncDisposable
{
    protected IAppLifeCycleService _appLifeCycleService = appLifeCycleService;
    public bool IsInitialized { get; set; }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized)
            throw new ApplicationException("It's been initialized");
        await InitializeAsync(true);
    }

    protected virtual async Task InitializeAsync(bool initialize)
    {
        IsInitialized = true;
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {

    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}
