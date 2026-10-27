using CommunityToolkit.Mvvm.ComponentModel;

namespace SpoofMess.Bases;

public partial class BaseViewModel : ObservableObject, IAsyncInitializable, IAsyncDisposable
{
    public bool IsInitialized { get; set; }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
            throw new ApplicationException("It's been initialized");
        await InitializeAsync();
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
