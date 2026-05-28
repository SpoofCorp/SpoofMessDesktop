using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpoofMess.ViewElements;

public partial class IconButton : UserControl
{
    private static readonly DependencyProperty TextProperty =
    DependencyProperty.Register(nameof(Text), typeof(string), typeof(IconButton));

    private static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(IconButton));

    private static readonly DependencyProperty AlignmentProperty =
        DependencyProperty.Register(nameof(Alignment), typeof(HorizontalAlignment), typeof(IconButton));

    private static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(IconButton));

    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(IconButton));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    public HorizontalAlignment Alignment
    {
        get => (HorizontalAlignment)GetValue(AlignmentProperty);
        set => SetValue(AlignmentProperty, value);
    }
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }
    public IconButton()
    {
        InitializeComponent();
    }
    private void InternalButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickEvent));
    }
}
