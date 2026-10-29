using CommonObjects.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpoofMess.Models;

public partial class Chat : ObservableObject, ISearchableEntity, IDisposable
{
    public Chat()
    {
        Messages.CollectionChanged += (sender, e) =>
            OnPropertyChanged(nameof(LastMessage));
        CurrentMessage = new() { ChatId = Id };
    }
    [ObservableProperty]
    public partial MessageModel CurrentMessage { get; set; }

    public Visibility EditedMessageVisibility => EditedMessage is null ? Visibility.Collapsed : Visibility.Visible;

    public MessageModel? EditedMessage
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(EditedMessageVisibility));
        }
    }

    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial string UniqueName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsPublic { get; set; }

    [ObservableProperty]
    public partial MessageModel? TargetMessage { get; set; }

    [ObservableProperty]
    public partial FileObject Avatar { get; set; }
    public double Position { get; set; }

    public MessageModel? LastMessage => Messages.LastOrDefault();

    public int ChatTypeId { get; set; }
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public ObservableCollection<FileObject> Avatars { get; } = [];

    public ConcurrentObservableCollection<MessageModel> Messages { get; } = [];

    public long RulesNumbers { get; set; }

    public bool IsOpened { get; set; }

    public ObservableCollection<PermissionResult> Rules { get; set; } = [];

    public ObservableCollection<ChatUser> Users { get; set; } = [];

    [ObservableProperty]
    public partial int CountUsers { get; set; }

    public Visibility ShowBan => Rules.Any(x => x.RuleId == (short)Permissions.Banning) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ShowKick => Rules.Any(x => x.RuleId == (short)Permissions.Kicking) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ShowMute => Rules.Any(x => x.RuleId == (short)Permissions.Muting) ? Visibility.Visible : Visibility.Collapsed;

    public void Dispose()
    {
        Messages.CollectionChanged -= (sender, e) =>
            OnPropertyChanged(nameof(LastMessage));
        GC.SuppressFinalize(this);
    }
}