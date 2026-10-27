namespace SpoofMess.Bases;

public interface IInitializable
{
    public bool IsInitialized { get; set; }
    public void Initialize();
}
