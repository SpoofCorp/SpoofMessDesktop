using CommonObjects.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace SpoofMess.Models;

public partial class Chat : ObservableObject
{
    public Chat()
    {
        Messages.CollectionChanged += (sender, e) => 
            OnPropertyChanged(nameof(LastMessage));
        CurrentMessage = new() { ChatId = Id };
    }
    [ObservableProperty]
    private MessageModel _currentMessage;
    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private string _uniqueName = string.Empty;
    [ObservableProperty]
    private bool _isPublic;

    public MessageModel? LastMessage => Messages.LastOrDefault();
    public int ChatTypeId { get; set; }
    public Guid Id { get; set; }
    public ObservableCollection<MessageModel> Messages { get; } = [];
    public ObservableCollection<PermissionResult> Rules { get; set; } = [];
}
