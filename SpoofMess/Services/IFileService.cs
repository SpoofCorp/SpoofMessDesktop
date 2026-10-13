using CommonObjects.Requests.Attachments;
using CommonObjects.Results;
using SpoofMess.Enums;
using SpoofMess.Models;
using System.IO;
using System.Net.Http;

namespace SpoofMess.Services;

public interface IFileService
{
    public string[]? GetFiles();

    public Result<FileObject> GetFile();

    public string[]? GetImages();

    public Result<FileObject> GetImage();

    public void Remove(FileObject file);

    public FileCategory GetCategory(Attachment attachment);

    public Task Save(FileObject file, CancellationToken cancellationToken = default);

    public Result<FileObject> GetFileInfo();

    public Result<FileObject> GetFileInfo(string? path);

    public Result<List<FileObject>> GetFilesInfo();

    public MultipartFormDataContent? GetStream(string? path);

    public string ToPrettySize(long bytes);

    public Stream? GetWriteStreamFromFile(string filePath);
    public Stream? GetReadStreamFromFile(string filePath);
}
