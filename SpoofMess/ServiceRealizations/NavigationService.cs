using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.ViewModels;
using SpoofMess.ViewModels.FileViewModels;
using SpoofMess.ViewModels.Settings;
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

    private TAdditionalViewModel GetAdditionalViewModel<TAdditionalViewModel>(ObservableObject owner, Action close) where TAdditionalViewModel : AdditionalViewModel
    {
        TAdditionalViewModel viewModel = _serviceProvider.GetRequiredService<TAdditionalViewModel>();
        viewModel.Initialize(owner, close);
        return viewModel;
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
