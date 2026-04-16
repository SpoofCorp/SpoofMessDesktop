using SpoofMess.Models;
using SpoofMess.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

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

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (ChatsList.SelectedItem is Chat chat && e.VerticalChange > 0)
        {
            chat.Position = e.VerticalOffset;
            Debug.WriteLine(chat.Position);
            Debug.WriteLine(e.VerticalOffset);
        }
    }

    private void ChatsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ChatsList.SelectedItem is Chat chat && MessagesList.Template.FindName("MessageScroll", MessagesList) is ScrollViewer scroll)
        {
            scroll.ScrollToVerticalOffset(chat.Position);
        }
    }
}
