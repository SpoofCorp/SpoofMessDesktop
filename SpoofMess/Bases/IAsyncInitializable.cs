namespace SpoofMess.Bases;

public interface IAsyncInitializable
{
    public bool IsInitialized { get; set; }

    public Task InitializeAsync();
}