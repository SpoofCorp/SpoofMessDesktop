using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;

namespace SpoofMess.ViewElements;

public partial class InputTextBox : UserControl
{
    private readonly static DependencyProperty HolderProperty =
        DependencyProperty.Register(
            nameof(Holder),
            typeof(string),
            typeof(InputTextBox),
            new FrameworkPropertyMetadata(
                default(string),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );

    private readonly static DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(InputTextBox),
            new FrameworkPropertyMetadata(
                default(string),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );

    private readonly static DependencyProperty AcceptsReturnProperty =
        DependencyProperty.Register(
            nameof(AcceptsReturn),
            typeof(bool),
            typeof(InputTextBox),
            new FrameworkPropertyMetadata(
                default(bool),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );

    private readonly static DependencyProperty TextWrappingProperty =
    DependencyProperty.Register(
        nameof(TextWrapping),
        typeof(TextWrapping),
        typeof(InputTextBox),
        new FrameworkPropertyMetadata(
            default(TextWrapping),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );


    public string Holder
    {
        get => (string)GetValue(HolderProperty);
        set => SetValue(HolderProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool AcceptsReturn
    {
        get => (bool)GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }
    public TextWrapping TextWrapping
    {
        get => (TextWrapping)GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    public InputTextBox()
    {
        InitializeComponent();
        TextWrapping = TextWrapping.NoWrap;
    }

    private void InputTextChanged(object sender, TextChangedEventArgs e)
    {
        HolderText.Visibility = string.IsNullOrEmpty(Input.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
