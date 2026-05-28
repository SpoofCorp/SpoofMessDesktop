using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SpoofMess.ViewElements;

public partial class InputPasswordBox : UserControl
{
    private readonly static DependencyProperty HolderProperty =
        DependencyProperty.Register(nameof(Holder), typeof(string), typeof(InputPasswordBox));

    private readonly static DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(InputPasswordBox),
            new FrameworkPropertyMetadata(
                default(string),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    private readonly static DependencyProperty WithShowingProperty =
        DependencyProperty.Register(nameof(WithShowing), typeof(Visibility), typeof(InputPasswordBox));

    private int SelectionStart;
    readonly StringBuilder builder = new();
    private TextBlock HolderText = null!;
    private TextBox Input = null!;
    public Visibility WithShowing
    {
        get => (Visibility)GetValue(WithShowingProperty);
        set => SetValue(WithShowingProperty, value);
    }

    private bool show;

    public string Holder
    {
        get =>
            (string)GetValue(HolderProperty);
        set =>
            SetValue(HolderProperty, value);
    }


    public string Text
    {
        get =>
            (string)GetValue(TextProperty);
        set =>
            SetValue(TextProperty, value);
    }

    public InputPasswordBox()
    {
        InitializeComponent();
    }

    private void InputTextChanged(object sender, TextChangedEventArgs e)
    {
        HolderText.Visibility = string.IsNullOrEmpty(Input.Text)
                   ? Visibility.Visible
                   : Visibility.Collapsed;
        Input.TextChanged -= InputTextChanged;
        SelectionStart = Input.SelectionStart;
        foreach (TextChange item in e.Changes)
        {
            if (item.RemovedLength > 0)
                builder.Remove(item.Offset, item.RemovedLength);
            if (item.AddedLength > 0)
                builder.Insert(item.Offset, Input.Text.AsSpan(item.Offset, item.AddedLength));
        }
        SetCurrentValue(TextProperty, builder.ToString());
        if (show)
            Input.Text = Text;
        else
            Input.Text = new string('*', Text.Length);
        Input.TextChanged += InputTextChanged;
        Input.SelectionStart = SelectionStart;
    }

    private void Field_Loaded(object sender, RoutedEventArgs e)
    {
        if (Field.Template.FindName(nameof(HolderText), Field) is TextBlock text)
            HolderText = text;
        else throw new Exception($"{nameof(HolderText)} doesn't not exists");
        if (Field.Template.FindName(nameof(Input), Field) is TextBox textBox)
            Input = textBox;
        else throw new Exception($"{nameof(Input)} doesn't not exists");
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
        Input.TextChanged -= InputTextChanged;
        show = !show;
        if (show)
            Input.Text = Text;
        else
            Input.Text = new string('*', Text.Length);
        Input.TextChanged += InputTextChanged;
    }
}
