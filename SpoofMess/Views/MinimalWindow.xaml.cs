using SpoofMess.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

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

    private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Grid { Tag: AdditionalViewModel additionalViewModel })
            additionalViewModel.CloseCommand.Execute(null);
    }

    private void ContentPresenter_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void Rectangle_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Rectangle { Tag: AdditionalViewModel additionalViewModel })
            additionalViewModel.CloseCommand.Execute(null);
        e.Handled = true;
    }
}
