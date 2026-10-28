using CommunityToolkit.Mvvm.Input;
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
    INavigationService navigationService,
    IAppLifeCycleService appLifeCycleService) : PlacementViewModel(appLifeCycleService), IAsyncInitializable
{
    private readonly IChatService _chatService = chatService;
    private readonly IChatUserService _chatUserService = chatUserService;
    private readonly ISearchService _searchService = searchService;
    private readonly INotificationApiService _notificationApiService = notificationApiService;
    private readonly INavigationService _navigationService = navigationService;
    public ConcurrentObservableCollection<Chat> Chats { get; set; } = [];

    public ObservableCollection<Chat> SearchResultsChats { get; set; } = [];

    public ObservableCollection<SearchableMessageWithChat> SearchResultsMessages { get; set; } = [];

    public Visibility ChatsVisibility => string.IsNullOrEmpty(Query) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility SearchResultsVisibility => string.IsNullOrEmpty(Query) ? Visibility.Collapsed : Visibility.Visible;

    public SearchableMessageWithChat? SelectedSearchableMessage
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SelectedSearchableMessage));
            GoToMessage(value);
        }
    }

    public Chat? SelectedChat
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(SelectedChat));
            OnSelectionChatChanged?.Invoke(value);
        }
    }

    public string Query
    {
        get;
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
        get;
        set
        {
            field = value;
            if (value is null)
            {
                OnPropertyChanged(nameof(SelectedSearchableEntity));
                return;
            }

            field = null;
            SelectChat(value);
            OnPropertyChanged(nameof(SelectedSearchableEntity));
            OnPropertyChanged(nameof(AdditionalViewModelVisibility));
        }
    }

    [RelayCommand]
    private void OpenInNewWindow(Chat chat)
    {
        _navigationService.OpenAdditionalWindow(chat);
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

        await _searchService.SimpleSearch(
            SearchResultsChats,
            SearchResultsMessages,
            Query,
            _appLifeCycleService.AppCancellationToken
        );
    }

    protected override async Task InitializeAsync(bool initialize)
    {
        _notificationApiService.OnChatCreated += _chatUserService.ChatCreated;
        _notificationApiService.OnChatUpdated += _chatService.Update;
        _notificationApiService.OnChatAvatarUpdated += _chatService.OnChatAvatarUpdated;
        ServiceRealizations.EventHandler.OnAdd += _chatService.OnMessageAdded;
        _chatService.ChatAdded += ChatAdded;
        _chatService.ChatMoved += ChatMoved;
        await _chatUserService.SyncChats(DateTime.UtcNow.AddMonths(-10));
        await base.InitializeAsync(initialize);
    }

    private async void SelectChat(Chat? searchableEntity)
    {
        if (searchableEntity is null) return;
        _navigationService.SetChatCardViewModel(this, searchableEntity);
    }

    private void ChatMoved(int arg1, int arg2)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Chat? selectedChat = SelectedChat;
            Chats.Move(arg1, arg2);
            SelectedChat = selectedChat;
        });
    }

    private void ChatAdded(Chat arg1, int? arg2)
    {
        if (Chats.Any(x => x.UniqueName == arg1.UniqueName))
            return;
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (arg2 is null)
                Chats.Add(arg1);
            else
                Chats.Insert(arg2.Value, arg1);

        });
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        ServiceRealizations.EventHandler.OnAdd -= _chatService.OnMessageAdded;
        _notificationApiService.OnChatAvatarUpdated -= _chatService.OnChatAvatarUpdated;
        _notificationApiService.OnChatUpdated -= _chatService.Update;
        _notificationApiService.OnChatCreated -= _chatUserService.ChatCreated;
        _chatService.ChatAdded -= ChatAdded;
        _chatService.ChatMoved -= ChatMoved;
        await base.DisposeAsync(disposing);
    }

    public void SetChat(Chat chat) =>
        SelectedChat = chat;

    public event SelectionChatChanged? OnSelectionChatChanged;

    public delegate void SelectionChatChanged(Chat? chat);
}
