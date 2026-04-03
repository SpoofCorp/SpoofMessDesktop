using CommonObjects.Results;
using SpoofFileParser;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace SpoofMess.ServiceRealizations;

public class DownloadService(IFileClassifier fileClassifier, IFileApiService fileApiService) : IDownloadService
{
    private readonly IFileClassifier _fileClassifier = fileClassifier;
    private readonly IFileApiService _fileApiService = fileApiService;
    private readonly ConcurrentDictionary<byte[], Task> _downloadTasks = new();
    private readonly ConcurrentDictionary<byte[], DownloadProgress> _progressMap = new();

    public async Task TryStart(FileObject file)
    {
        DownloadProgress progress = _progressMap.GetOrAdd(file.Id!, _ => new());

        progress.OnChanged += (percent) =>
        {
            file.DownloadPercent = percent;
        };
        if (string.IsNullOrEmpty(progress.Path))
            progress.Path = file.Path!;
        else
            file.Path = progress.Path;
        await _downloadTasks.GetOrAdd(file.Id!, id => Task.Run(async () =>
        {
            try
            {
                var streamResult = await _fileApiService.Upload(file.Token);
                if (streamResult.Success)
                {
                    await Download(file, progress, streamResult.Body!);
                }
            }
            finally
            {
                _downloadTasks.TryRemove(file.Id!, out _);
                _progressMap.TryRemove(file.Id!, out _);
            }
        }));
    }

    public async Task Download(FileObject file, DownloadProgress progress, Stream input)
    {
        string directory = (Path.HasExtension(file.Path) ? Path.GetDirectoryName(file.Path) : file.Path) ?? Guid.CreateVersion7().ToString();
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        file.Path = Path.Combine(directory, file.Name ?? (Path.HasExtension(file.Path) ? Path.GetFileName(file.Path) : "Undefined"));
        progress.Path = file.Path;
        if (File.Exists(file.Path))
        {
            progress.Raise(100);
            input.Dispose();
            return;
        }
        await using (FileStream fileStream = new(
            file.Path,
            FileMode.CreateNew))
        {
            byte[] buffer = new byte[8192];
            long totalRead = 0;
            int bytesRead;
            long totalBytes = file.Size;
            while ((bytesRead = await input.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                totalRead += bytesRead;
                if (totalBytes > 0)
                {
                    progress.Raise((double)totalRead / totalBytes * 100);
                }
            }
        }
        FileExtension extension = _fileClassifier.GetExtension(file.Path);

        progress.Raise(100);
        input.Dispose();

        progress.Clear();
    }
}
