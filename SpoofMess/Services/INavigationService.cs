using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models;
using SpoofMess.ViewModels;
using SpoofMess.ViewModels.FileViewModels;
using SpoofMess.ViewModels.Settings;

namespace SpoofMess.Services;

public interface INavigationService
{
    public void OpenWindow();
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
}
