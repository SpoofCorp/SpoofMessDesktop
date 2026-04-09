using System.ComponentModel;
using System.Windows;

namespace SpoofMess.Views;
public partial class CentralView : Window
{
    public CentralView()
    {
        InitializeComponent();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown(0);
    }
}
