using CommunityToolkit.Mvvm.ComponentModel;

namespace SpoofMess.Models;

public partial class PlayableFileObject : FileObject
{
    [ObservableProperty]
    public partial bool IsPlayed { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentPositionValue))]
    public partial TimeSpan CurrentPosition { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DurationValue))]
    public partial TimeSpan Duration { get; set; }
    public double CurrentPositionValue
    {
        get => CurrentPosition.TotalMicroseconds;
        set
        {
            value = Math.Clamp(value, 0, DurationValue);
            IsSeeking = true;
            CurrentPosition = TimeSpan.FromMicroseconds(value);
        }
    }
    public bool IsSeeking { get; set; }
    public double DurationValue => Duration.TotalMicroseconds;

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