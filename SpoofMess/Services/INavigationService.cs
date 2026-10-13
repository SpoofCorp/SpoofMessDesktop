using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.ViewModels;
using SpoofMess.ViewModels.FileViewModels;
using SpoofMess.ViewModels.Notifications;
using SpoofMess.ViewModels.Settings;

namespace SpoofMess.Services;

public interface INavigationService : IAsyncInitializable
{
    public bool IsActive { get; }

    public void ShowNotification(MessageModel message);

    public void OpenWindow();

    public void OpenAdditionalWindow(Chat chat);
    public void CloseAdditionalWindow(string key);

    public StaticNotificationViewModel GetAudioService();

    public void CloseWindow();

    public void ResizeWindow();

    public void HideToTrayWindow();

    public void HideWindow();

    public void ShowCentralViewWithMain();

    public void ShowCentralViewWithAuthorization();

    public void ShowCentralView();

    public void ShowMainView();

    public void ShowRegistrationView();

    public void ShowAuthorizationView();

    public ImageViewModel GetImageViewModel(FileObject file);

    public MusicViewModel GetMusicViewModel(FileObject file);

    public FileViewModel GetFileViewModel(FileObject file);

    public ProfileViewModel GetProfileViewModel();

    public void SetSettingsViewModel(IPlacementViewModel owner);

    public void SetProfileViewModel(IPlacementViewModel owner);

    public void SetAdvancedViewModel(IPlacementViewModel owner);

    public void SetCreateGroupViewModel(IPlacementViewModel owner);

    public void SetLanguageViewModel(IPlacementViewModel owner);

    public void SetSoundsAndNotificationsViewModel(IPlacementViewModel owner);

    public void SetDesignViewModel(IPlacementViewModel owner);

    public void SetChatMemberCard(IPlacementViewModel owner, ChatUser chatUser);

    public Task SetChatInfoViewModel(IPlacementViewModel owner, Chat chat);

    public void SetChatCardViewModel(IPlacementViewModel owner, Chat chat);
}
