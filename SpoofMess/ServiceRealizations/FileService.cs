using CommonObjects.Requests.Attachments;
using CommonObjects.Results;
using Microsoft.Win32;
using SpoofFileParser;
using SpoofMess.Enums;
using SpoofMess.Models;
using SpoofMess.Services;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SpoofMess.ServiceRealizations;

public class FileService(IFileClassifier fileClassifier, IDownloadService downloadService) : IFileService
{
    private static readonly string[] Units = ["B", "KB", "MB", "GB", "TB", "PB"];
    private readonly IFileClassifier _fileClassifier = fileClassifier;
    private readonly IDownloadService _downloadService = downloadService;

    private readonly static string _imageFilter = "Все изображения|*.jpg;*.jpeg;*.png;*.webp;*.heic;*.heif;*.bmp;*.gif;*.tiff;*.tif|JPEG файлы (*.jpg, *.jpeg)|*.jpg;*.jpeg|PNG файлы (*.png)|*.png|WebP файлы (*.webp)|*.webp|HEIC/HEIF файлы (*.heic, *.heif)|*.heic;*.heif|GIF файлы (*.gif)|*.gif";

    public string[]? GetFiles() =>
        GetMany();

    public Result<FileObject> GetFile() =>
        GetFileInfo(GetOnce());
    public Result<List<FileObject>> GetFilesInfo()
    {
        IEnumerable<Result<FileObject>> result = (GetMany() ?? []).Select(x => GetFileInfo(x));
        if (result.Any(x => !x.Success))
            return Result<List<FileObject>>.ErrorResult("Okak");
        return Result<List<FileObject>>.OkResult(result.Select(x => x.Body!).ToList());
    }

    public string[]? GetImages() =>
        GetMany(_imageFilter);

    public Result<FileObject> GetImage() =>
        GetFileInfo(GetOnce(_imageFilter));

    private static string[]? GetMany(string? filter = null)
    {
        OpenFileDialog fileDialog = new()
        {
            Multiselect = true,
        };
        if (filter is not null)
            fileDialog.Filter = filter;
        if (fileDialog.ShowDialog() is true)
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

    public MultipartFormDataContent? GetStream(string? path)
    {
        if (!File.Exists(path))
            return null;

        MultipartFormDataContent form = [];
        FileStream fileStream = File.OpenRead(path);
        StreamContent fileContent = new(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        form.Add(fileContent, "file", Path.GetFileName(path));
        return form;
    }
    public async Task Save(FileObject file)
    {
        await _downloadService.TryStart(file);
    }

    public FileCategory GetCategory(Attachment attachment)
    {
        if (Enum.TryParse(attachment.Category, true, out FileCategory category))
            return category;
        return FileCategory.File;
    }

    public Result<FileObject> GetFileInfo()
    {
        return GetFileInfo(GetOnce());
    }

    private Result<FileObject> GetFileInfo(string? path)
    {
        if (path is null)
            return Result<FileObject>.BadRequest("Not selected");
        FileExtension extension = _fileClassifier.GetExtension(path);
        if (extension == default)
            return Result<FileObject>.NotFoundResult("Unexpected file format");
        FileObject file = new()
        {
            ExtensionId = extension.Id,
            Name = Path.GetFileName(path),
            Path = path,
            Size = extension.Size,
        };
        if (Enum.TryParse(extension.Type.ToString(), true, out FileCategory category))
            file.Category = category;

        return Result<FileObject>.OkResult(file);
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

    public Stream? GetObjectFromFile(string filePath)
    {
        if (File.Exists(filePath))
            return File.OpenRead(filePath);
        return null;
    }
}
