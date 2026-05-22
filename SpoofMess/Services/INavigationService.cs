using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models;
using SpoofMess.ViewModels;
using SpoofMess.ViewModels.FileViewModels;
using SpoofMess.ViewModels.Settings;

namespace SpoofMess.Services;

public interface INavigationService
{
    public void OpenWindow();
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
    public SettingsViewModel GetSettingsViewModel(ObservableObject owner, Action close);
    public ProfileViewModel GetProfileViewModel();
    public CreateGroupViewModel GetCreateGroupViewModel(ObservableObject owner, Action close);
    public AdvancedViewModel GetAdvancedViewModel(ObservableObject owner, Action close);
    public ProfileViewModel GetProfileViewModel(ObservableObject owner, Action close);
    public LanguageViewModel GetLanguageViewModel(ObservableObject owner, Action close);
    public DesingViewModel GetDesignViewModel(ObservableObject owner, Action close);
    public ChatInfoViewModel GetChatInfoViewModel(ObservableObject owner, Action close, Chat chat);
    public ChatCardViewModel GetChatCardViewModel(ObservableObject owner, Action close, Chat chat);
}
