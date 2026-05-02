using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpoofMess.ViewModels;

public partial class ChatsViewModel(
    IChatService chatService,
    IChatUserService chatUserService,
    INotificationApiService notificationApiService) : ObservableObject, IInitializable, IDisposable
{
    private readonly IChatService _chatService = chatService;
    private readonly IChatUserService _chatUserService = chatUserService;
    private readonly INotificationApiService _notificationApiService = notificationApiService;

    public ObservableCollection<Chat> Chats { get; set; } = [];

    public Chat? SelectedChat
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SelectedChat));
            OnSelectionChatChanged?.Invoke(value);
        }
    }

    [ObservableProperty]
    private string _query = string.Empty;

    [RelayCommand]
    private void Search()
    {
        if (string.IsNullOrEmpty(Query))
            return;

    }

    public async Task InitializeAsync()
    {
        _notificationApiService.OnChatUpdated += _chatService.Update;
        ServiceRealizations.EventHandler.OnAdd += _chatService.OnMessageAdded;
        _chatService.ChatAdded += ChatAdded;
        _chatService.ChatMoved += ChatMoved;
        await _chatUserService.SyncChats(DateTime.UtcNow.AddMonths(-10));
    }

    private void ChatMoved(int arg1, int arg2)
    {
        lock (Chats)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Chat? selectedChat = SelectedChat;
                Chats.Move(arg1, arg2);
                SelectedChat = selectedChat;
            });
        }
    }

    private void ChatAdded(Chat arg1, int? arg2)
    {
        lock (Chats)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (arg2 is null)
                    Chats.Add(arg1);
                else
                    Chats.Insert(arg2.Value, arg1);

            });
        }
    }

    public void Dispose()
    {
        ServiceRealizations.EventHandler.OnAdd -= _chatService.OnMessageAdded;
        _notificationApiService.OnChatUpdated -= _chatService.Update;
        _chatService.ChatAdded -= ChatAdded;
        _chatService.ChatMoved -= ChatMoved;
        GC.SuppressFinalize(this);
    }

    public event SelectionChatChanged? OnSelectionChatChanged;

    public delegate void SelectionChatChanged(Chat? chat);
}
