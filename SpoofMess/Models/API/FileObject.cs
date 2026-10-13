using CommonObjects.Requests.Attachments;
using CommunityToolkit.Mvvm.ComponentModel;
using SpoofFileParser.FileMetadata;
using SpoofMess.Enums;
using System.Text.Json.Serialization;

namespace SpoofMess.Models;

public partial class FileObject : ObservableObject
{
    [ObservableProperty]
    public partial double DownloadPercent { get; set; }
    [ObservableProperty]
    public partial string? Path { get; set; }

    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial long Size { get; set; }
    [ObservableProperty]
    public partial short ExtensionId { get; set; }

    [ObservableProperty]
    public partial byte[] Thumbnail { get; set; } = [];

    [ObservableProperty]
    [JsonIgnore]
    public partial string PrettySize { get; set; } = string.Empty;

    public byte[]? Id;

    public byte[]? Token;

    public byte[]? AttachmentToken;

    public IFileMetadata Metadata { get; set; } = null!;

    public IAttachmentMetadata? AttachmentMetadata { get; set; }

    public FileCategory Category { get; set; }
}
