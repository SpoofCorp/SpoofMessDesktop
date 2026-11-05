namespace SpoofMess.Services;

public interface IAppLifeCycleService
{
    public Task OnStartupAsync();

    public Task OnExitAsync();
    public Task OnCloseAsync(bool isTerminate = false);
    public CancellationToken AppCancellationToken { get; }
}
