using CommunityToolkit.Mvvm.ComponentModel;
using SpoofFileParser.FileMetadata;
using SpoofMess.Enums;
using System.Text.Json.Serialization;

namespace SpoofMess.Models;

public partial class FileObject : ObservableObject
{
    [ObservableProperty]
    private double _downloadPercent;
    [ObservableProperty]
    private string? _path;
    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private long _size;
    [ObservableProperty]
    private short _extensionId;
    [ObservableProperty]
    public byte[] _thumbnail = [];
    [ObservableProperty]
    [JsonIgnore]
    private string _prettySize = string.Empty;
    public byte[]? Id;

    public byte[]? Token;
    public byte[]? AttachmentToken;
    public IFileMetadata Metadata { get; set; } = null!;
    public FileCategory Category { get; set; }
}
