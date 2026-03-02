using Microsoft.Extensions.DependencyInjection;
using SpoofMess.Services;
using SpoofMess.ViewModels;
using SpoofMess.Views;
using System.Windows;

namespace SpoofMess.ServiceRealizations;

public class NavigationService(
        IServiceProvider serviceProvider
    ) : INavigationService
{
    private Window _currentMainWindow = null!;
    private Window? _currentSlaveWindow;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public void ShowEntryView() =>
        ShowMainWindow<EntryWindow, EntryViewModel>();

    public void ShowMainView() =>
        ShowMainWindow<MainView, MainViewModel>();


    private TView GetMainWindow<TView, TViewModel>() where TView : Window where TViewModel : class
    {
        TView view = _serviceProvider.GetService<TView>()
            ?? throw new ApplicationException($"Not found DI of {typeof(TView)}");
        TViewModel viewModel = _serviceProvider.GetService<TViewModel>()
            ?? throw new ApplicationException($"Not found DI of {typeof(TView)}");

        view.DataContext = viewModel;
        return view;
    }

    private void ShowMainWindow<TView, TViewModel>() where TView : Window where TViewModel : class
    {
        TView view = GetMainWindow<TView, TViewModel>();
        _currentMainWindow?.Close();
        view.Show();
        _currentMainWindow = view;
    }
}
