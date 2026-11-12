using AdditionalHelpers.Services;
using CommonObjects.Requests.Attachments;
using CommonObjects.Results;
using Microsoft.Win32;
using SpoofFileParser;
using SpoofMess.Enums;
using SpoofMess.Models;
using SpoofMess.Services;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SpoofMess.ServiceRealizations;

public class FileService(IFileClassifier fileClassifier, IDownloadService downloadService, ISerializer serializer) : IFileService
{
    private static readonly string[] Units = ["B", "KB", "MB", "GB", "TB", "PB"];
    private readonly IFileClassifier _fileClassifier = fileClassifier;
    private readonly IDownloadService _downloadService = downloadService;
    private readonly ISerializer _serializer = serializer;
    private ConcurrentDictionary<string, SemaphoreSlim> fileSemaphores = [];

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
        return Result<List<FileObject>>.OkResult([.. result.Select(x => x.Body!)]);
    }

    public string[]? GetImages() =>
        GetMany(_imageFilter);

    public Result<FileObject> GetImage() =>
        GetFileInfo(GetOnce(_imageFilter));

    private static string[]? GetMany(string? filter = null)
    {
        OpenFileDialog fileDialog = GetFileDialog(true, filter);
        if (fileDialog.ShowDialog() is true)
            return fileDialog.FileNames;

        return null;
    }
    private static string? GetOnce(string? filter = null)
    {
        OpenFileDialog fileDialog = GetFileDialog(false, filter);
        if (fileDialog.ShowDialog() is true)
            return fileDialog.FileName;

        return null;
    }

    private static OpenFileDialog GetFileDialog(bool multiSelect, string? filter = null)
    {
        OpenFileDialog fileDialog = new()
        {
            Multiselect = multiSelect,
        };
        if (filter is not null)
            fileDialog.Filter = filter;

        return fileDialog;
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

    public void Remove(FileObject file)
    {
        if (File.Exists(file.Path))
        {
            string directory = (Path.HasExtension(file.Path) ? Path.GetDirectoryName(file.Path) : file.Path) ?? Guid.CreateVersion7().ToString();
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            file.Path = Path.Combine(directory, file.Name ?? (Path.HasExtension(file.Path) ? Path.GetFileName(file.Path) : "Undefined"));
            lock (file.Path)
            {
                //File.Delete(file.Path);
            }
        }
    }

    public async Task Save(FileObject file, CancellationToken cancellationToken = default)
    {
        await _downloadService.TryStart(file, cancellationToken);
    }

    public FileCategory GetCategory(Attachment attachment)
    {
        if (Enum.TryParse(attachment.Category, true, out FileCategory category))
            return category;
        return FileCategory.File;
    }

    public Result<FileObject> GetFileInfo() =>
        GetFileInfo(GetOnce());

    public Result<FileObject> GetFileInfo(string? path)
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

    public async Task SaveToFile<T>(string path, T content, CancellationToken cancellationToken = default)
    {
        SemaphoreSlim semaphore = fileSemaphores.GetOrAdd(path, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            string directoryPath = Path.GetDirectoryName(path)!;
            string tempPath = $"{path}.tmp";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            Stream? stream = null;
            try
            {
                stream = GetWriteStreamFromFile(tempPath);
                if (stream is null)
                    return;
                await _serializer.Serialize(
                    content,
                    stream,
                    cancellationToken
                );
                await stream.FlushAsync(cancellationToken);
                await stream.DisposeAsync();
                stream = null;
                if (File.Exists(tempPath))
                    File.Move(tempPath, path, true);
            }
            finally
            {
                if (stream is not null)
                    await stream.DisposeAsync();
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<Result<T>> GetFromFile<T>(string path, CancellationToken cancellationToken = default)
    {
        SemaphoreSlim semaphore = fileSemaphores.GetOrAdd(path, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);
        T data;
        try
        {
            Stream? stream = GetReadStreamFromFile(path);
            if (stream is null) return Result<T>.ErrorResult("File is null");
            try
            {
                data = await _serializer.Deserialize<T>(stream, cancellationToken);
            }
            catch (JsonException)
            {
                return Result<T>.ErrorResult("Invalid json");
            }
            finally
            {
                await stream.DisposeAsync();
            }
        }
        finally
        {
            semaphore.Release();
        }
        return Result<T>.OkResult(data);
    }

    public Stream? GetReadStreamFromFile(string filePath)
    {
        if (File.Exists(filePath))
            return File.OpenRead(filePath);
        return null;
    }
    public Stream? GetWriteStreamFromFile(string filePath) =>
        new FileStream(filePath, FileMode.OpenOrCreate);
}
