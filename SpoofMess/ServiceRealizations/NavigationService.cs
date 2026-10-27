using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.ViewModels;
using SpoofMess.ViewModels.FileViewModels;
using SpoofMess.ViewModels.Notifications;
using SpoofMess.ViewModels.Settings;
using SpoofMess.Views;
using System.Collections.Concurrent;
using System.Windows;

namespace SpoofMess.ServiceRealizations;

public class NavigationService(
        IServiceProvider serviceProvider,
        NotificationInfoViewModel notificationInfoViewModel,
        UserInfo userInfo,
        IAppLifeCycleService appLifeCycleService
    ) : INavigationService
{
    public bool IsActive { get; set; }
    public bool IsInitialized { get; set; }

    public Window CurrentMainWindow
    {
        get;
        set
        {
            if (field is not null)
            {
                field.Activated -= CurrentMainWindow_Activated;
                field.Deactivated -= CurrentMainWindow_Deactivated;
            }
            field = value;
            if (value is not null)
            {
                field.Activated += CurrentMainWindow_Activated;
                field.Deactivated += CurrentMainWindow_Deactivated;
            }
        }
    } = null!;
    private CentralViewModel _currentViewModel = null!;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    protected NotificationView? NotificationWindow;
    protected Lock NotificationWindowLock = new();
    protected NotificationInfoViewModel _notificationInfoViewModel = notificationInfoViewModel;
    private readonly UserInfo _userInfo = userInfo;
    private readonly ConcurrentDictionary<string, AdditionalWindowWithDataContext> _windows = [];

    public StaticNotificationViewModel GetAudioService() =>
        _serviceProvider.GetRequiredService<StaticNotificationViewModel>();

    public void OpenAdditionalWindow(Chat chat)
    {
        AdditionalWindowWithDataContext additionalWindowWithDataContext = _windows.GetOrAdd(chat.UniqueName, (key) =>
        {
            var dataContext = _serviceProvider.GetRequiredService<MessagesViewModel>();
            dataContext.SelectedChat = chat;
            AdditionalWindowWithDataContext additionalWindowWithDataContext = new(key, _serviceProvider.GetRequiredService<INavigationService>());
            additionalWindowWithDataContext.SetDataContext(dataContext);
            additionalWindowWithDataContext.SetWindow(new MinimalWindow());
            return additionalWindowWithDataContext;
        });
        additionalWindowWithDataContext.Start();
    }

    public void OpenWindow()
    {
        CurrentMainWindow.Show();
        CurrentMainWindow.Activate();
    }

    public void CloseWindow()
    {
        CurrentMainWindow.Close();
    }

    public void CloseAdditionalWindow(string key)
    {
        _windows.TryRemove(key, out _);
    }

    public void OpenChatWindow(Chat chat)
    {
        Window window;
        if (_userInfo.CloneWindows)
        {

        }
        else
        {
            MessagesViewModel viewModel = _serviceProvider.GetRequiredService<MessagesViewModel>();
            viewModel.SelectedChat = chat;
            window = new()
            {
                DataContext = viewModel
            };
            window.Show();
        }
    }

    public void ResizeWindow()
    {
        CurrentMainWindow.WindowState = CurrentMainWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    public void HideWindow()
    {
        CurrentMainWindow.WindowState = WindowState.Minimized;
    }

    public void HideToTrayWindow()
    {
        CurrentMainWindow.Hide();
    }

    public void ShowCentralView()
    {
        CurrentMainWindow = _serviceProvider.GetRequiredService<CentralView>();
        _currentViewModel = GetViewModel<CentralViewModel>();
        CurrentMainWindow.DataContext = _currentViewModel;
        CurrentMainWindow.Show();
    }

    public void ShowCentralViewWithMain() =>
        ShowCentralViewWithViewModel<MainViewModel>();

    public void ShowCentralViewWithAuthorization() =>
        ShowCentralViewWithViewModel<AuthorizationViewModel>();

    public void ShowAuthorizationView() =>
        _currentViewModel.View = GetViewModel<AuthorizationViewModel>();

    public void ShowRegistrationView() =>
        _currentViewModel.View = GetViewModel<RegistrationViewModel>();

    public void ShowMainView() =>
        _currentViewModel.View = GetViewModel<MainViewModel>();

    public FileViewModel GetFileViewModel(FileObject file) =>
        GetFileViewModel<FileViewModel>(file);

    public void SetSettingsViewModel(IPlacementViewModel owner) =>
        SetAdditionalViewModel<SettingsViewModel>(owner);

    public void SetProfileViewModel(IPlacementViewModel owner) =>
        SetAdditionalViewModel<ProfileViewModel>(owner);

    public void SetAdvancedViewModel(IPlacementViewModel owner) =>
        SetAdditionalViewModel<AdvancedViewModel>(owner);

    public ProfileViewModel GetProfileViewModel() =>
        _serviceProvider.GetRequiredService<ProfileViewModel>();

    public void SetCreateGroupViewModel(IPlacementViewModel owner) =>
        SetAdditionalViewModel<CreateGroupViewModel>(owner);

    public void SetLanguageViewModel(IPlacementViewModel owner) =>
        SetAdditionalViewModel<LanguageViewModel>(owner);

    public void SetSoundsAndNotificationsViewModel(IPlacementViewModel owner) =>
        SetAdditionalViewModel<SoundsAndNotificationsViewModel>(owner);

    public void SetDesignViewModel(IPlacementViewModel owner) =>
        SetAdditionalViewModel<DesingViewModel>(owner);

    public void SetChatMemberCard(IPlacementViewModel owner, ChatUser chatUser)
    {
        ChatMemberCardViewModel viewModel = GetAdditionalViewModel<ChatMemberCardViewModel>(owner);
        viewModel.ChatUser = chatUser;
    }

    public async Task SetChatInfoViewModel(IPlacementViewModel owner, Chat chat)
    {
        ChatInfoViewModel viewModel = _serviceProvider.GetRequiredService<ChatInfoViewModel>();
        viewModel.Chat = chat;
        await viewModel.InitializeAsync();
        owner.ShowAdditionalViewModel(viewModel);
    }

    public MusicViewModel GetMusicViewModel(FileObject file) =>
        GetFileViewModel<MusicViewModel>(file);

    public ImageViewModel GetImageViewModel(FileObject file) =>
        GetFileViewModel<ImageViewModel>(file);


    private TFileViewModel GetFileViewModel<TFileViewModel>(FileObject file) where TFileViewModel : ObjectViewModel
    {
        TFileViewModel imageViewModel = GetViewModel<TFileViewModel>();
        imageViewModel.Add(file);
        return imageViewModel;
    }

    private void SetAdditionalViewModel<TAdditionalViewModel>(IPlacementViewModel owner) where TAdditionalViewModel : AdditionalViewModel
    {
        TAdditionalViewModel viewModel = GetViewModel<TAdditionalViewModel>();
        owner.ShowAdditionalViewModel(viewModel);
    }

    private TAdditionalViewModel GetAdditionalViewModel<TAdditionalViewModel>(IPlacementViewModel owner) where TAdditionalViewModel : AdditionalViewModel
    {
        TAdditionalViewModel viewModel = GetViewModel<TAdditionalViewModel>();
        owner.ShowAdditionalViewModel(viewModel);
        return viewModel;
    }

    private void ShowCentralViewWithViewModel<TViewModel>() where TViewModel : ObservableObject
    {
        CurrentMainWindow = _serviceProvider.GetRequiredService<CentralView>();
        _currentViewModel = GetViewModel<CentralViewModel>();
        CurrentMainWindow.DataContext = _currentViewModel;
        _currentViewModel.View = GetViewModel<TViewModel>();
        CurrentMainWindow.Show();
        if (_currentViewModel.View is IAsyncInitializable asyncInitializable)
            asyncInitializable.InitializeAsync();
        if (_currentViewModel.View is IInitializable initializable)
            initializable.InitializeAsync();
    }

    private TViewModel GetViewModel<TViewModel>() where TViewModel : ObservableObject
    {
        TViewModel viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        if (viewModel is IAsyncInitializable initializable)
            initializable.InitializeAsync();
        return viewModel;
    }

    public void SetChatCardViewModel(IPlacementViewModel owner, Chat chat)
    {
        ChatCardViewModel chatCardViewModel = GetAdditionalViewModel<ChatCardViewModel>(owner);
        chatCardViewModel.Chat = chat;
    }

    public void ShowNotification(MessageModel message)
    {
        if (NotificationWindow is null)
        {
            lock (NotificationWindowLock)
            {
                if (NotificationWindow is not null)
                    return;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    NotificationWindow = new() { DataContext = _notificationInfoViewModel };
                    NotificationWindow.Show();
                });
            }
        }
        _notificationInfoViewModel.Add(message);
    }



    private void CurrentMainWindow_Deactivated(object? sender, EventArgs e) =>
        IsActive = false;

    private void CurrentMainWindow_Activated(object? sender, EventArgs e) =>
        IsActive = true;

    public async Task InitializeAsync()
    {

    }
}
