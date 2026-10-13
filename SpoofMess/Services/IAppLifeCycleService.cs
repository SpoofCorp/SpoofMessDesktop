namespace SpoofMess.Services;

public interface IAppLifeCycleService
{
    public Task OnStartupAsync();

    public Task OnExitAsync();
}
