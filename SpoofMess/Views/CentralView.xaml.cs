using SpoofMess.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SpoofMess.Views;

public partial class CentralView : Window
{
    public CentralView()
    {
        InitializeComponent();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        NotifyIcon.Dispose();
        if (DataContext is CentralViewModel cvm)
            cvm.CloseCommand.Execute(default);
    }

    private void DockPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}
