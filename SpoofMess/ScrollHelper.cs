using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SpoofMess;

public static class ScrollHelper
{
    public static readonly DependencyProperty VerticalOffsetProperty =
        DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollHelper),
            new PropertyMetadata(0.0, OnVerticalOffsetChanged));


    public static readonly DependencyProperty ScrollToItemProperty =
        DependencyProperty.RegisterAttached("ScrollToItem", typeof(object), typeof(ScrollHelper), new PropertyMetadata(null, OnScrollToItemChanged));

    public static void SetScrollToItem(DependencyObject obj, object value) => obj.SetValue(ScrollToItemProperty, value);
    public static object GetScrollToItem(DependencyObject obj) => obj.GetValue(ScrollToItemProperty);

    public static void SetVerticalOffset(DependencyObject target, double value) => target.SetValue(VerticalOffsetProperty, value);
    public static double GetVerticalOffset(DependencyObject target) => (double)target.GetValue(VerticalOffsetProperty);

    private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ScrollViewer viewer)
        {
            viewer.ScrollToVerticalOffset((double)e.NewValue);
        }
    }

    private static void OnScrollToItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ItemsControl itemsControl && e.NewValue is not null)
        {
            if (itemsControl.ItemContainerGenerator.ContainerFromItem(e.NewValue) is not UIElement container)
                return;
            int index = itemsControl.Items.IndexOf(e.NewValue);
            if (index < 0)
            {
                SetScrollToItem(itemsControl, null!);
                return;
            }
            itemsControl.Dispatcher.BeginInvoke(() =>
            {

                ScrollViewer? scrollViewer = FindVisualChild<ScrollViewer>(itemsControl);
                if (scrollViewer is null) return;
                VirtualizingPanel? virtualizingPanel = FindVisualChild<VirtualizingPanel>(scrollViewer);
                if (virtualizingPanel is null)
                    ((FrameworkElement)container).BringIntoView();
                else
                    virtualizingPanel.BringIndexIntoViewPublic(index);

                SetScrollToItem(itemsControl, null!);
            }, System.Windows.Threading.DispatcherPriority.Send);
        }
    }
    private static T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            if (child is T target)
                return target;

            if (FindVisualChild<T>(child) is T childOfChild)
                return childOfChild;
        }
        return null;
    }
}