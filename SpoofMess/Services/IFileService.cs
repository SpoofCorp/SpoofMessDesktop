namespace SpoofMess.Services;

public interface IFileService
{
    public string[]? GetFiles();

    public string? GetFile();

    public string[]? GetImages();

    public string? GetImage();
}
