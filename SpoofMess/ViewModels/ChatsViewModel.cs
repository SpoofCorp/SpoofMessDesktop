using CommonObjects.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpoofMess.ViewModels;

public partial class ChatsViewModel(
    IChatService chatService,
    IChatUserService chatUserService,
    ISearchService searchService,
    INotificationApiService notificationApiService,
    INavigationService navigationService) : ObservableObject, IInitializable, IDisposable
{
    private readonly IChatService _chatService = chatService;
    private readonly IChatUserService _chatUserService = chatUserService;
    private readonly ISearchService _searchService = searchService;
    private readonly INotificationApiService _notificationApiService = notificationApiService;
    private readonly INavigationService _navigationService = navigationService;

    public ObservableCollection<Chat> Chats { get; set; } = [];

    public ObservableCollection<Chat> SearchResultsChats { get; set; } = [];

    public ObservableCollection<SearchableMessageWithChat> SearchResultsMessages { get; set; } = [];

    public Visibility ChatsVisibility => string.IsNullOrEmpty(Query) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility SearchResultsVisibility => string.IsNullOrEmpty(Query) ? Visibility.Collapsed : Visibility.Visible;

    public Visibility AdditionalViewModelVisibility => AdditionalViewModel is null ? Visibility.Collapsed : Visibility.Visible;

    [NotifyPropertyChangedFor(nameof(AdditionalViewModelVisibility))]
    [ObservableProperty]
    private AdditionalViewModel? _additionalViewModel;

    public SearchableMessageWithChat? SelectedSearchableMessage
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SelectedSearchableMessage));
            GoToMessage(value);
        }
    }

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

    public string Query
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Query));
            OnPropertyChanged(nameof(ChatsVisibility));
            OnPropertyChanged(nameof(SearchResultsVisibility));
            Search();
        }
    } = string.Empty;

    public Chat? SelectedSearchableEntity
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SelectedSearchableEntity));
            OnPropertyChanged(nameof(AdditionalViewModelVisibility));
            SelectChat(value);
        }
    }

    private void GoToMessage(SearchableMessageWithChat? searchableMessageWithChat)
    {
        if (searchableMessageWithChat is null)
            return;

        MessageModel? message = searchableMessageWithChat.Chat.Messages.FirstOrDefault(x => x.Id == searchableMessageWithChat.Id);
        if (message is null)
            return;
        searchableMessageWithChat.Chat.TargetMessage = message;
        SelectedChat = searchableMessageWithChat.Chat;
    }

    private async void Search()
    {
        if (string.IsNullOrEmpty(Query))
            return;

        await _searchService.SimpleSearch(SearchResultsChats, SearchResultsMessages, Query);
    }

    public async void InitializeAsync()
    {
        _notificationApiService.OnChatCreated += _chatUserService.ChatCreated;
        _notificationApiService.OnChatUpdated += _chatService.Update;
        _notificationApiService.OnChatAvatarUpdated += _chatService.OnChatAvatarUpdated;
        ServiceRealizations.EventHandler.OnAdd += _chatService.OnMessageAdded;
        _chatService.ChatAdded += ChatAdded;
        _chatService.ChatMoved += ChatMoved;
        await _chatUserService.SyncChats(DateTime.UtcNow.AddMonths(-10));
    }

    private async void SelectChat(Chat? searchableEntity)
    {
        if (searchableEntity is null) return;
        AdditionalViewModel = _navigationService.GetChatCardViewModel(this, () => AdditionalViewModel = null, searchableEntity);
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

    private void Close() =>
        AdditionalViewModel = null;

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
        _notificationApiService.OnChatAvatarUpdated -= _chatService.OnChatAvatarUpdated;
        _notificationApiService.OnChatUpdated -= _chatService.Update;
        _notificationApiService.OnChatCreated -= _chatUserService.ChatCreated;
        _chatService.ChatAdded -= ChatAdded;
        _chatService.ChatMoved -= ChatMoved;
        GC.SuppressFinalize(this);
    }

    public void SetChat(Chat chat) =>
        SelectedChat = chat;

    public event SelectionChatChanged? OnSelectionChatChanged;

    public delegate void SelectionChatChanged(Chat? chat);
}
