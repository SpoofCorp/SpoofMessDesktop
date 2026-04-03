using SpoofMess.Models;
using System.IO;

namespace SpoofMess.Services;

public interface IDownloadService
{
    public Task TryStart(FileObject file);

    public Task Download(FileObject file, DownloadProgress progress, Stream input);
}
