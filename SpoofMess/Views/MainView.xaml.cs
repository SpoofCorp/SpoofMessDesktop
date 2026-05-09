using SpoofMess.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace SpoofMess.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void Menu_Click(object sender, RoutedEventArgs e)
    {
        SideMenu.ChangeMenuVisibility();
    }

    private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Grid { Tag: AdditionalViewModel additionalViewModel})
            additionalViewModel.CloseCommand.Execute(null);
    }
    private void Menu_Item_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Grid { Tag: AdditionalViewModel additionalViewModel })
            additionalViewModel.CloseCommand.Execute(null);
        SideMenu.ChangeMenuVisibility();
        e.Handled = true;
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
