using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Models;
using SpoofMess.ViewModels;
using SpoofMess.ViewModels.FileViewModels;
using SpoofMess.ViewModels.Notifications;
using SpoofMess.ViewModels.Settings;
using SpoofMess.Views;
using System.Windows;

namespace SpoofMess.ServiceRealizations;

public class NavigationService(
        IServiceProvider serviceProvider,
        NotificationInfoViewModel notificationInfoViewModel
    ) : INavigationService
{
    public bool IsActive { get; set; }

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

    public StaticNotificationViewModel GetAudioService() =>
        _serviceProvider.GetRequiredService<StaticNotificationViewModel>();

    public void OpenWindow()
    {
        CurrentMainWindow.Show();
        CurrentMainWindow.Activate();
    }

    public void CloseWindow()
    {
        CurrentMainWindow.Close();
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

    public SettingsViewModel GetSettingsViewModel(ObservableObject owner, Action close) =>
        GetAdditionalViewModel<SettingsViewModel>(owner, close);

    public ProfileViewModel GetProfileViewModel(ObservableObject owner, Action close) =>
        GetAdditionalViewModel<ProfileViewModel>(owner, close);


    public AdvancedViewModel GetAdvancedViewModel(ObservableObject owner, Action close) =>
        GetAdditionalViewModel<AdvancedViewModel>(owner, close);

    public ProfileViewModel GetProfileViewModel() =>
        _serviceProvider.GetRequiredService<ProfileViewModel>();

    public CreateGroupViewModel GetCreateGroupViewModel(ObservableObject owner, Action close) =>
        GetAdditionalViewModel<CreateGroupViewModel>(owner, close);

    public LanguageViewModel GetLanguageViewModel(ObservableObject owner, Action close) =>
        GetAdditionalViewModel<LanguageViewModel>(owner, close);

    public SoundsAndNotificationsViewModel GetSoundsAndNotificationsViewModel(ObservableObject owner, Action close) =>
        GetAdditionalViewModel<SoundsAndNotificationsViewModel>(owner, close);

    public DesingViewModel GetDesignViewModel(ObservableObject owner, Action close) =>
        GetAdditionalViewModel<DesingViewModel>(owner, close);

    public async Task<ChatInfoViewModel> GetChatInfoViewModel(ObservableObject owner, Action close, Chat chat)
    {
        ChatInfoViewModel viewModel = GetAdditionalViewModel<ChatInfoViewModel>(owner, close);
        viewModel.Chat = chat;
        await viewModel.InitiailizeAsync();
        return viewModel;
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

    private TAdditionalViewModel GetAdditionalViewModel<TAdditionalViewModel>(ObservableObject owner, Action close) where TAdditionalViewModel : AdditionalViewModel
    {
        TAdditionalViewModel viewModel = GetViewModel<TAdditionalViewModel>();
        viewModel.Initialize(owner, close);
        return viewModel;
    }

    private void ShowCentralViewWithViewModel<TViewModel>() where TViewModel : ObservableObject
    {
        CurrentMainWindow = _serviceProvider.GetRequiredService<CentralView>();
        _currentViewModel = GetViewModel<CentralViewModel>();
        CurrentMainWindow.DataContext = _currentViewModel;
        _currentViewModel.View = GetViewModel<TViewModel>();
        CurrentMainWindow.Show();
        if (_currentViewModel.View is IInitializable initializable)
            initializable.InitializeAsync();
    }

    private TViewModel GetViewModel<TViewModel>() where TViewModel : ObservableObject
    {
        TViewModel viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        return viewModel;
    }

    public ChatCardViewModel GetChatCardViewModel(ObservableObject owner, Action close, Chat chat)
    {
        ChatCardViewModel chatCardViewModel = new(chat, _serviceProvider.GetRequiredService<IChatService>(), _serviceProvider.GetRequiredService<IChatUserService>(), _serviceProvider.GetRequiredService<IMessageService>());
        chatCardViewModel.Initialize(owner, close);
        return chatCardViewModel;
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

    public void InitializeAsync()
    {
    }
}
