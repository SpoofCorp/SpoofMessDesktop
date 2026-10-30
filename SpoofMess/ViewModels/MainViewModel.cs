using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using System.Windows;

namespace SpoofMess.ViewModels;

public partial class MainViewModel(
    INotificationService notificationService,
    INotificationApiService notificationApiService,
    MainMessagesViewModel messagesViewModel,
    INavigationService navigationService,
    IUserService userService,
    IAuthService authService,
    ChatsViewModel chatsViewModel,
    UserInfo userInfo,
    IAppLifeCycleService appLifeCycleService) : PlacementViewModel(appLifeCycleService), IAsyncInitializable
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

    [ObservableProperty]
    public partial Visibility SideMenuVisibility { get; set; } = Visibility.Collapsed;

    [RelayCommand]
    private void ShowProfile()
    {
        _navigationService.SetProfileViewModel(this);
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
            await _authService.Save(_appLifeCycleService.AppCancellationToken);
            _navigationService.CloseWindow();
        }
    }

    protected override async Task InitializeAsync(bool initialize)
    {
        try
        {
            MessagesViewModel.SetGetChatInfo(async (chat) => await _navigationService.SetChatInfoViewModel(this, chat, _appLifeCycleService.AppCancellationToken));
            _notificationApiService.OnUserUpdated += _userService.OnUserUpdated;
            ChatsViewModel.OnSelectionChatChanged += MessagesViewModel.OnSelectionChatChanged;
            ServiceRealizations.EventHandler.OnUserAvatarUpdated += _userService.AvatarUpdateHandler;
            await Task.WhenAll(ChatsViewModel.InitializeAsync(), MessagesViewModel.InitializeAsync());
            await _notificationApiService.InitializeAsync();
            await base.InitializeAsync(initialize);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError(ex.Message);
        }
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        token.Cancel();
        _notificationApiService.OnUserUpdated -= _userService.OnUserUpdated;
        ServiceRealizations.EventHandler.OnUserAvatarUpdated -= _userService.AvatarUpdateHandler;
        ChatsViewModel.OnSelectionChatChanged -= MessagesViewModel.OnSelectionChatChanged;
        await base.DisposeAsync(disposing);
    }
}