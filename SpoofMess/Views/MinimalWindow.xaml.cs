using System.Windows;
using System.Windows.Input;

namespace SpoofMess.Views;

public partial class MinimalWindow : Window
{
    public MinimalWindow()
    {
        InitializeComponent();
    }

    private void DockPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}
