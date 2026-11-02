using CommunityToolkit.Mvvm.ComponentModel;

namespace SpoofMess.Models;

public partial class PlayableFileObject : FileObject
{
    [ObservableProperty]
    public partial bool IsPlayed { get; set; }

    public PlayableFileObject(FileObject fileObject)
    {
        DownloadPercent = fileObject.DownloadPercent;
        Path = fileObject.Path;
        Name = fileObject.Name;
        Size = fileObject.Size;
        ExtensionId = fileObject.ExtensionId;
        Thumbnail = fileObject.Thumbnail;
        PrettySize = fileObject.PrettySize;
        Id = fileObject.Id;
        Token = fileObject.Token;
        AttachmentToken = fileObject.AttachmentToken;
        Metadata = fileObject.Metadata;
        Category = fileObject.Category;
    }
}