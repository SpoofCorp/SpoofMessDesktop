using SpoofMess.ViewModels;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace SpoofMess.Views;

public partial class MainViewTest : UserControl
{
    public MainViewTest()
    {
        InitializeComponent();
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
