using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.ViewModels.Notifications;
using System.Windows;

namespace SpoofMess.ViewModels;

public partial class MainViewModel(
    INotificationService notificationService,
    INotificationApiService notificationApiService,
    MainMessagesViewModel messagesViewModel,
    INavigationService navigationService,
    StaticNotificationViewModel staticNotificationViewModel,
    IUserService userService,
    IAuthService authService,
    ChatsViewModel chatsViewModel,
    UserInfo userInfo) : PlacementViewModel, IDisposable, IAsyncInitializable
{
    private readonly CancellationTokenSource token = new();
    private readonly UserInfo _userInfo = userInfo;
    private readonly INotificationApiService _notificationApiService = notificationApiService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IUserService _userService = userService;
    private readonly IAuthService _authService = authService;
    private readonly INavigationService _navigationService = navigationService;

    public MainViewModel() : this(default,
                                  default,
                                  default,
                                  default,
                                  default,
                                  default,
                                  default,
                                  default,
                                  default)
    {

    }

    public ChatsViewModel ChatsViewModel { get; set; } = chatsViewModel;

    public MessagesViewModel MessagesViewModel { get; set; } = messagesViewModel;
    public StaticNotificationViewModel StaticNotificationViewModel { get; set; } = staticNotificationViewModel;

    [ObservableProperty]
    public partial Visibility SideMenuVisibility { get; set; } = Visibility.Collapsed;
    public bool IsInitialized { get; set; }

    [RelayCommand]
    private void ShowProfile()
    {
        //_navigationService.SetProfileViewModel(this);
    }

    [RelayCommand]
    private void ShowSettings()
    {
        _navigationService.SetSettingsViewModel(this);
    }

    [RelayCommand]
    private void ShowCreateGroup()
    {
        _navigationService.SetCreateGroupViewModel(this);
    }

    [RelayCommand]
    private void ShowMemberCard(ChatUser chatUser)
    {
        _navigationService.SetChatMemberCard(this, chatUser);
    }

    public async Task CloseWindow()
    {
        if (_userInfo.HideToTray)
        {
            _navigationService.HideToTrayWindow();
            if (_userInfo.UnselectChat)
                ChatsViewModel.SelectedChat = null;
            BehindCloseAdditionalViewModel();
            SideMenuVisibility = Visibility.Collapsed;
        }
        else
        {
            await _authService.Save();
            _navigationService.CloseWindow();
        }
    }

    public async Task InitializeAsync()
    {
        try
        {
            MessagesViewModel.SetGetChatInfo(async (chat) => await _navigationService.SetChatInfoViewModel(this, chat));
            _notificationApiService.OnUserUpdated += _userService.OnUserUpdated;
            ChatsViewModel.OnSelectionChatChanged += MessagesViewModel.OnSelectionChatChanged;
            ServiceRealizations.EventHandler.OnUserAvatarUpdated += _userService.AvatarUpdateHandler;
            await Task.WhenAll(ChatsViewModel.InitializeAsync(), MessagesViewModel.InitializeAsync());
            await _notificationApiService.InitializeAsync();
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
        ServiceRealizations.EventHandler.OnUserAvatarUpdated -= _userService.AvatarUpdateHandler;
        ChatsViewModel.OnSelectionChatChanged -= MessagesViewModel.OnSelectionChatChanged;
        GC.SuppressFinalize(this);
    }
}