namespace SpoofMess;

public class DownloadProgress
{
    public bool IsStarted { get; set; }

    public string Path { get; set; } = string.Empty;

    public event Action<double>? OnChanged;

    public void Raise(double percent) => OnChanged?.Invoke(percent);

    public void Clear() => OnChanged = null;
}