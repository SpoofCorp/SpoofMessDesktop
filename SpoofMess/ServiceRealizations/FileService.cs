using Microsoft.Win32;
using SpoofMess.Services;

namespace SpoofMess.ServiceRealizations;

public class FileService : IFileService
{
    private readonly static string _imageFilter = "Все изображения|*.jpg;*.jpeg;*.png;*.webp;*.heic;*.heif;*.bmp;*.gif;*.tiff;*.tif|JPEG файлы (*.jpg, *.jpeg)|*.jpg;*.jpeg|PNG файлы (*.png)|*.png|WebP файлы (*.webp)|*.webp|HEIC/HEIF файлы (*.heic, *.heif)|*.heic;*.heif|GIF файлы (*.gif)|*.gif|";

    public string[]? GetFiles() => 
        GetMany();

    public string? GetFile() => 
        GetOnce();

    public string[]? GetImages() =>
        GetMany(_imageFilter);

    public string? GetImage() => 
        GetOnce(_imageFilter);

    private static string[]? GetMany(string? filter = null)
    {
        OpenFileDialog fileDialog = new()
        {
            Multiselect = true,
        };
        if(filter is not null)
            fileDialog.Filter = filter;
        if(fileDialog.ShowDialog() is true)
            return fileDialog.FileNames;

        return null;
    }
    private static string? GetOnce(string? filter = null)
    {
        OpenFileDialog fileDialog = new()
        {
            Multiselect = false,
        };
        if (filter is not null)
            fileDialog.Filter = filter;
        if (fileDialog.ShowDialog() is true)
            return fileDialog.FileName;

        return null;
    }
}
