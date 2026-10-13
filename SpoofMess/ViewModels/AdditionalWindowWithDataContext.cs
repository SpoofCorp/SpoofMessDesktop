using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Services;
using System.Windows;

namespace SpoofMess.ViewModels;

public partial class AdditionalWindowWithDataContext(string key, INavigationService navigation)
{
    private string Key { get; set; } = key;

    private Window Window { get; set; } = null!;

    public ObservableObject DataContext { get; set; } = null!;

    private bool isStarted = false;
    private readonly INavigationService _navigation = navigation;

    public void SetWindow(Window window)
    {
        Window?.Close();
        Window = window;
        Window.DataContext = this;
    }

    public void SetDataContext(ObservableObject dataContext)
    {
        DataContext = dataContext;
    }

    public void Start()
    {
        if (isStarted)
        {
            Window.Focus();
            return;
        }
        if (DataContext is IInitializable initializable)
            initializable.InitializeAsync();
        Window.Show();
        isStarted = true;
    }

    [RelayCommand]
    public void Hide()
    {
        Window.Hide();
    }

    [RelayCommand]
    public void Close()
    {
        Window.Close();
        _navigation.CloseAdditionalWindow(Key);
    }
}
