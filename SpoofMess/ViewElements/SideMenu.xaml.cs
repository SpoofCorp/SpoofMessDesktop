using System.Windows;
using System.Windows.Controls;

namespace SpoofMess.ViewElements;

public partial class SideMenu : UserControl
{
    private static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
        nameof(IsOpen),
        typeof(Visibility),
        typeof(SideMenu),
        new FrameworkPropertyMetadata(
            default(Visibility),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public Visibility IsOpen
    {
        get => (Visibility)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public SideMenu()
    {
        InitializeComponent();
    }

    public void ChangeMenuVisibility()
    {
        SetCurrentValue(IsOpenProperty, SideMenuView.Visibility is Visibility.Visible
            ? Visibility.Collapsed
            : Visibility.Visible);
    }

    private void Rectangle_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        SetCurrentValue(IsOpenProperty, Visibility.Collapsed);
    }
}
