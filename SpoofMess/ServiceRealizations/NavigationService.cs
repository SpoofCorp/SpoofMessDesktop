using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.ViewModels;
using SpoofMess.ViewModels.FileViewModels;
using SpoofMess.Views;
using System.Windows;

namespace SpoofMess.ServiceRealizations;

public class NavigationService(
        IServiceProvider serviceProvider
    ) : INavigationService
{
    private Window _currentMainWindow = null!;
    private CentralViewModel _currentViewModel = null!;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public void OpenWindow()
    {
        _currentMainWindow.Show();
        _currentMainWindow.Activate();
    }
    public void ShowCentralView()
    {
        _currentMainWindow = _serviceProvider.GetRequiredService<CentralView>();
        _currentViewModel = _serviceProvider.GetRequiredService<CentralViewModel>();
        _currentMainWindow.DataContext = _currentViewModel;
        _currentMainWindow.Show();
    }

    public void ShowCentralViewWithMain() => 
        ShowCentralViewWithViewModel<MainViewModel>();

    public void ShowCentralViewWithAuthorization() => 
        ShowCentralViewWithViewModel<AuthorizationViewModel>();

    public void ShowAuthorizationView() =>
        _currentViewModel.View = _serviceProvider.GetRequiredService<AuthorizationViewModel>();

    public void ShowRegistrationView() =>
        _currentViewModel.View = _serviceProvider.GetRequiredService<RegistrationViewModel>();

    public void ShowMainView() =>
        _currentViewModel.View = _serviceProvider.GetRequiredService<MainViewModel>();

    public FileViewModel GetFileViewModel(FileObject file) =>
        GetFileViewModel<FileViewModel>(file);

    public SettingsViewModel GetSettingsViewModel(Action close)
    {
        SettingsViewModel viewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
        viewModel.SetClose(close);
        return viewModel;
    }

    public ProfileViewModel GetProfileViewModel() =>
        _serviceProvider.GetRequiredService<ProfileViewModel>();

    public CreateGroupViewModel GetCreateGroupViewModel(Action close)
    {
        CreateGroupViewModel viewModel = _serviceProvider.GetRequiredService<CreateGroupViewModel>();
        viewModel.SetClose(close);
        return viewModel;
    }

    public MusicViewModel GetMusicViewModel(FileObject file) =>
        GetFileViewModel<MusicViewModel>(file);

    public ImageViewModel GetImageViewModel(FileObject file) =>
        GetFileViewModel<ImageViewModel>(file);


    private TFileViewModel GetFileViewModel<TFileViewModel>(FileObject file) where TFileViewModel : ObjectViewModel
    {
        TFileViewModel imageViewModel = _serviceProvider.GetRequiredService<TFileViewModel>();
        imageViewModel.Files.Add(file);
        return imageViewModel;
    }

    private void ShowCentralViewWithViewModel<TViewModel>() where TViewModel : ObservableObject
    {
        _currentMainWindow = _serviceProvider.GetRequiredService<CentralView>();
        _currentViewModel = _serviceProvider.GetRequiredService<CentralViewModel>();
        _currentMainWindow.DataContext = _currentViewModel;
        _currentViewModel.View = _serviceProvider.GetRequiredService<TViewModel>();
        _currentMainWindow.Show();
    }
}
