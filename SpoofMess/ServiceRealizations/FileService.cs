using CommonObjects.Requests.Attachments;
using CommonObjects.Results;
using Microsoft.Win32;
using SpoofFileInfo;
using SpoofMess.Enums;
using SpoofMess.Models;
using SpoofMess.Services;
using System.IO;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations;

public class FileService(IFileClassifier fileClassifier) : IFileService
{
    private static readonly string[] Units = { "B", "KB", "MB", "GB", "TB", "PB" };
    private readonly IFileClassifier _fileClassifier = fileClassifier;

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

    public MultipartFormDataContent GetStream(string path)
    {

        MultipartFormDataContent form = [];
        FileStream fileStream = File.OpenRead(path);
        StreamContent fileContent = new(fileStream);

        form.Add(fileContent, "file", Path.GetFileName(path));
        return form;
    }
    public async Task Save(Stream input, FileObject file)
    {
        string directory = file.Path ?? Guid.NewGuid().ToString();
        if(!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, file.Name ?? "Undefined");
        file.Path = path;
        if (File.Exists(path))
            return;
        await using var fileStream = new FileStream(
            path,
            FileMode.CreateNew);
        await input.CopyToAsync(fileStream);
    }

    public FileCategory GetCategory(Attachment attachment)
    {
        if (Enum.TryParse(attachment.Category, true, out FileCategory category))
            return category;
        return FileCategory.File;
    }

    public Result<FileObject> GetFileInfo()
    {
        string? path = GetOnce();
        if (path is null)
            return Result<FileObject>.BadRequest("Not selected");
        FileExtension2 extension2 = _fileClassifier.GetExtension(path);
        if (extension2 == default)
            return Result<FileObject>.NotFoundResult("Unexpected file format");

        return Result<FileObject>.OkResult(new()
        {
            ExtensionId = extension2.Id,
            Name = Path.GetFileName(path),
            Path = path,
            Size = extension2.Size
        });
    }

    public string ToPrettySize(long bytes)
    {
        double size = bytes;
        int unitIndex = 0;

        while (size >= 1024 && unitIndex < Units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }
        return $"{size:0.##} {Units[unitIndex]}";
    }
}
