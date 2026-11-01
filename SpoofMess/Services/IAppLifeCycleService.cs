namespace SpoofMess.Services;

public interface IAppLifeCycleService
{
    public Task OnStartupAsync();

    public Task OnExitAsync();
    public Task OnCloseAsync();
    public CancellationToken AppCancellationToken { get; }
}
