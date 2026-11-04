using SpoofMess.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace SpoofMess.ViewElements;

public partial class ModalView : UserControl
{
    public static readonly DependencyProperty AdditionalViewModelProperty =
        DependencyProperty.Register(
            nameof(AdditionalViewModel),
            typeof(AdditionalViewModel),
            typeof(ModalView),
            new FrameworkPropertyMetadata(
                default(AdditionalViewModel),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
            )
        );

    public AdditionalViewModel AdditionalViewModel
    {
        get => (AdditionalViewModel)GetValue(AdditionalViewModelProperty);
        set => SetValue(AdditionalViewModelProperty, value);
    }

    public ModalView()
    {
        InitializeComponent();
    }

    private void Rectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Rectangle { Tag: AdditionalViewModel additionalViewModel })
            additionalViewModel.CloseCommand.Execute(null);
        e.Handled = true;
    }

    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Grid { Tag: AdditionalViewModel additionalViewModel })
            additionalViewModel.CloseCommand.Execute(null);
    }
    private void ContentPresenter_MouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }
}
