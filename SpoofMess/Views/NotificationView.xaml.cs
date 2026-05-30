using System.Windows;

namespace SpoofMess.Views;

public partial class NotificationView : Window
{
    public NotificationView()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Top = SystemParameters.PrimaryScreenHeight - ActualHeight;
        Left = SystemParameters.PrimaryScreenWidth - 200;
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if(e.HeightChanged)
            Top = SystemParameters.PrimaryScreenHeight - ActualHeight;
    }
}
