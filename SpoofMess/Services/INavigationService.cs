using SpoofMess.Models;
using SpoofMess.ViewModels.FileViewModels;

namespace SpoofMess.Services;

public interface INavigationService
{
    public void ShowMainView();
    public void ShowEntryView();
    public void GetRegistrationViewModel();
    public void GetAuthorizationViewModel();
    public ImageViewModel GetImageViewModel(FileObject file);
    public MusicViewModel GetMusicViewModel(FileObject file);
    public FileViewModel GetFileViewModel(FileObject file);
}
