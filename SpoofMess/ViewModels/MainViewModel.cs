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
    MessagesViewModel messagesViewModel,
    INavigationService navigationService,
    StaticNotificationViewModel staticNotificationViewModel,
    IUserService userService,
    IAuthService authService,
    ChatsViewModel chatsViewModel,
    UserInfo userInfo) : ObservableObject, IDisposable, IInitializable
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
    public StaticNotificationViewModel StaticNotificationViewModel { get; set; }


    [NotifyPropertyChangedFor(nameof(AdditionalVisibility))]
    [ObservableProperty]
    public partial AdditionalViewModel? AdditionalView { get; set; }

    [ObservableProperty]
    public partial Visibility SideMenuVisibility { get; set; } = Visibility.Collapsed;

    public Visibility AdditionalVisibility => AdditionalView is null ? Visibility.Collapsed : Visibility.Visible;

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
    private void ShowCreateGroup()
    {
        AdditionalView = _navigationService.GetCreateGroupViewModel(this, Close);
    }

    private void Close() =>
        AdditionalView = null;

    public async Task CloseWindow()
    {
        if (_userInfo.HideToTray)
        {
            _navigationService.HideToTrayWindow();
            if (_userInfo.UnselectChat)
                ChatsViewModel.SelectedChat = null;
            AdditionalView?.CloseCommand.Execute(null);
            AdditionalView = null;
            SideMenuVisibility = Visibility.Collapsed;
        }
        else
        {
            await _authService.Save();
            _navigationService.CloseWindow();
        }
    }

    public async void InitializeAsync()
    {
        try
        {
            MessagesViewModel.SetGetChatInfo(() => AdditionalView = _navigationService.GetChatInfoViewModel(this, Close, MessagesViewModel.SelectedChat!));
            _notificationApiService.OnUserUpdated += _userService.OnUserUpdated;
            ChatsViewModel.OnSelectionChatChanged += MessagesViewModel.OnSelectionChatChanged;
            ServiceRealizations.EventHandler.OnUserAvatarUpdated += _userService.AvatarUpdateHandler;
            ChatsViewModel.InitializeAsync();
            MessagesViewModel.InitializeAsync();
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