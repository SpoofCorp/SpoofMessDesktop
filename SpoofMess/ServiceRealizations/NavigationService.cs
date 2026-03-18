using Microsoft.Extensions.DependencyInjection;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.ViewModels;
using SpoofMess.ViewModels.FileViewModels;
using SpoofMess.Views;
using System.Windows;
using System.Windows.Controls;

namespace SpoofMess.ServiceRealizations;

public class NavigationService(
        IServiceProvider serviceProvider
    ) : INavigationService
{
    private Window _currentMainWindow = null!;
    private Window? _currentSlaveWindow;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public void ShowEntryView()
    {
        SetMainWindow<EntryWindow, EntryViewModel>();
        GetAuthorizationViewModel();
        _currentMainWindow.Show();
    }

    public void ShowMainView() =>
        ShowMainWindow<MainView, MainViewModel>();

    public void GetRegistrationViewModel() =>
        ChangeView<RegistrationViewModel>();

    public void GetAuthorizationViewModel() =>
        ChangeView<AuthorizationViewModel>();

    public MusicViewModel GetMusicViewModel(FileObject file) =>
        GetFileViewModel<MusicViewModel>(file);


    public ImageViewModel GetImageViewModel(FileObject file) =>
        GetFileViewModel<ImageViewModel>(file);

    private TFileViewModel GetFileViewModel<TFileViewModel>(FileObject file) where TFileViewModel : FileViewModel
    {
        TFileViewModel imageViewModel = GetViewModel<TFileViewModel>();
        imageViewModel.Files.Add(file);
        return imageViewModel;
    }

    private TViewModel GetViewModel<TViewModel>() where TViewModel : class =>
        _serviceProvider.GetRequiredService<TViewModel>();

    private void ChangeView<TViewModel>() where TViewModel : class
    {
        TViewModel viewModel = GetViewModel<TViewModel>();
        if (_currentMainWindow.DataContext is EntryViewModel evm)
            evm.ViewModel = viewModel;
    }

    private TView GetView<TView, TViewModel>() where TView : ContentControl where TViewModel : class
    {
        TView view = _serviceProvider.GetRequiredService<TView>();
        TViewModel viewModel = _serviceProvider.GetRequiredService<TViewModel>();

        view.DataContext = viewModel;
        return view;
    }

    private void ShowMainWindow<TView, TViewModel>() where TView : Window where TViewModel : class
    {
        TView view = GetView<TView, TViewModel>();
        _currentMainWindow?.Close();
        view.Show();
        _currentMainWindow = view;
    }
    private void SetMainWindow<TView, TViewModel>() where TView : Window where TViewModel : class
    {
        TView view = GetView<TView, TViewModel>();
        _currentMainWindow?.Close();
        _currentMainWindow = view;
    }

    public FileViewModel GetFileViewModel(FileObject file) =>
        GetFileViewModel<FileViewModel>(file);

    public SettingsViewModel GetSettingsViewModel() =>
        GetViewModel<SettingsViewModel>();
    public ProfileViewModel GetProfileViewModel() =>
        GetViewModel<ProfileViewModel>();
}
