using SpoofMess.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpoofMess.Views;

public partial class CentralView : Window
{
    public CentralView()
    {
        InitializeComponent();
    }

    private async void Close_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button closeButton) closeButton.IsEnabled = false;
            IsEnabled = false;
        NotifyIcon.Dispose();
        if (DataContext is CentralViewModel cvm)
            await cvm.CloseApp();
    }

    private void DockPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}
