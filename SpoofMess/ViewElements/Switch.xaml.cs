using System.Windows;
using System.Windows.Controls;

namespace SpoofMess.ViewElements;

public partial class Switch : UserControl
{
    private static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
        nameof(IsChecked),
        typeof(bool),
        typeof(Switch),
        new FrameworkPropertyMetadata(
            default(bool),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    private readonly static DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(Switch));


    public string Text
    {
        get =>
            (string)GetValue(TextProperty);
        set =>
            SetValue(TextProperty, value);
    }

    public bool IsChecked
    {
        get =>
            (bool)GetValue(IsCheckedProperty);
        set =>
            SetValue(IsCheckedProperty, value);
    }

    public Switch()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        SetCurrentValue(IsCheckedProperty, !IsChecked);
    }
}
