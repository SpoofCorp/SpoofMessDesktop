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

    public string? GetFile();

    public string[]? GetImages();

    public string? GetImage();

    public FileCategory GetCategory(Attachment attachment);

    public Task Save(Stream input, FileObject file);

    public Result<FileObject> GetFileInfo();

    public MultipartFormDataContent GetStream(string path);

    public string ToPrettySize(long bytes);
}
