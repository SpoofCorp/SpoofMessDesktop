using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace SpoofMess.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable, IInitializable
{
    private readonly CancellationTokenSource token = new();
    private readonly UserInfo _userInfo;
    private readonly INotificationApiService _notificationApiService;
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;
    private readonly IChatService _chatService;
    private readonly INavigationService _navigationService;

    public ChatsViewModel ChatsViewModel { get; set; }

    public MessagesViewModel MessagesViewModel { get; set; }

    public ObservableCollection<Chat> Chats { get; set; }


    [NotifyPropertyChangedFor(nameof(AdditionalVisibility))]
    [ObservableProperty]
    private AdditionalViewModel? _additionalView;

    [ObservableProperty]
    private Visibility _sideMenuVisibility = Visibility.Collapsed;

    public Visibility AdditionalVisibility => AdditionalView is null ? Visibility.Collapsed : Visibility.Visible;

    public MainViewModel() { }

    public MainViewModel(
        INotificationService notificationService,
        INotificationApiService notificationApiService,
        MessagesViewModel messagesViewModel,
        INavigationService navigationService,
        IChatService chatService,
        IUserService userService,
        ChatsViewModel chatsViewModel,
        UserInfo userInfo)
    {
        _notificationService = notificationService;
        _notificationApiService = notificationApiService;
        MessagesViewModel = messagesViewModel;
        _navigationService = navigationService;
        _chatService = chatService;
        _userInfo = userInfo;
        _userService = userService;
        ChatsViewModel = chatsViewModel;
        _notificationApiService.OnUserUpdated += _userService.OnUserUpdated;
        Chats = _chatService.Chats;
    }


    [RelayCommand]
    private void ShowProfile()
    {
        AdditionalView = _navigationService.GetProfileViewModel(this, Close);
    }

    [RelayCommand]
    private void ShowSettings()
    {
        AdditionalView = _navigationService.GetSettingsViewModel(this, Close);
    }

    [RelayCommand]
    private void ShowChatInfo()
    {
        if (MessagesViewModel.SelectedChat is not null)
            AdditionalView = _navigationService.GetChatInfoViewModel(this, Close, MessagesViewModel.SelectedChat);
    }

    [RelayCommand]
    private void ShowCreateGroup()
    {
        AdditionalView = _navigationService.GetCreateGroupViewModel(this, Close);
    }

    private void Close() =>
        AdditionalView = null;

    public void Close(Window window, CancelEventArgs e)
    {
        if (_userInfo.HideToTray)
        {
            window.Hide();
            if (_userInfo.UnselectChat)
                MessagesViewModel.SelectedChat = null;
            AdditionalView?.CloseCommand.Execute(null);
            AdditionalView = null;
            e.Cancel = true;
            SideMenuVisibility = Visibility.Collapsed;
        }
    }

    public async Task InitializeAsync()
    {
        try
        {
            await ChatsViewModel.InitializeAsync();
            await MessagesViewModel.InitializeAsync();
            ChatsViewModel.OnSelectionChatChanged += MessagesViewModel.OnSelectionChatChanged;
        }
        catch (Exception ex)
        {
            _notificationService.ShowError(ex.Message);
        }
    }

    public void Dispose()
    {
        token.Cancel();
        _notificationApiService.OnUserUpdated -= _userService.OnUserUpdated;
        ChatsViewModel.OnSelectionChatChanged -= MessagesViewModel.OnSelectionChatChanged;
        GC.SuppressFinalize(this);
    }
}