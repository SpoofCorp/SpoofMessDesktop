using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SpoofMess;

public static class ScrollHelper
{
    public static readonly DependencyProperty VerticalOffsetProperty =
        DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollHelper),
            new PropertyMetadata(0.0, OnVerticalOffsetChanged));

    public static readonly DependencyProperty ScrollToItemProperty =
        DependencyProperty.RegisterAttached("ScrollToItem", typeof(object), typeof(ScrollHelper), new PropertyMetadata(null, OnScrollToItemChanged));

    public static readonly DependencyProperty ScrollTopCommandProperty =
    DependencyProperty.RegisterAttached("ScrollTopCommand", typeof(ICommand), typeof(ScrollHelper),
        new PropertyMetadata(null, OnScrollCommandsChanged));

    public static readonly DependencyProperty ScrollBottomCommandProperty =
        DependencyProperty.RegisterAttached("ScrollBottomCommand", typeof(ICommand), typeof(ScrollHelper),
            new PropertyMetadata(null, OnScrollCommandsChanged));
    public static void SetScrollTopCommand(DependencyObject obj, ICommand value) => obj.SetValue(ScrollTopCommandProperty, value);
    public static ICommand GetScrollTopCommand(DependencyObject obj) => (ICommand)obj.GetValue(ScrollTopCommandProperty);

    public static void SetScrollBottomCommand(DependencyObject obj, ICommand value) => obj.SetValue(ScrollBottomCommandProperty, value);
    public static ICommand GetScrollBottomCommand(DependencyObject obj) => (ICommand)obj.GetValue(ScrollBottomCommandProperty);

    public static void SetScrollToItem(DependencyObject obj, object value) => obj.SetValue(ScrollToItemProperty, value);
    public static object GetScrollToItem(DependencyObject obj) => obj.GetValue(ScrollToItemProperty);

    public static void SetVerticalOffset(DependencyObject target, double value) => target.SetValue(VerticalOffsetProperty, value);
    public static double GetVerticalOffset(DependencyObject target) => (double)target.GetValue(VerticalOffsetProperty);


    private static void OnScrollCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            if (!element.IsLoaded)
            {
                element.Loaded += Viewer_Loaded;
                return;
            }
            Attach(element);
        }
    }

    private static void Viewer_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not ScrollViewer viewer)
            return;
        viewer.Loaded -= Viewer_Loaded;
        Attach(viewer);
    }

    private static void Attach(FrameworkElement element)
    {
        ScrollViewer? scrollViewer = element is ScrollViewer viewer
            ? viewer
            : FindVisualChild<ScrollViewer>(element);
        if (scrollViewer is null) return;

        scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
        scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
    }

    private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is not ScrollViewer scrollViewer)
            return; 
        DependencyObject target = FindTargetWithCommands(scrollViewer);
        if (target == null) return;

        if (scrollViewer.VerticalOffset == 0 && e.VerticalChange < 0)
        {
            ICommand topCommand = GetScrollTopCommand(target);
            if (topCommand?.CanExecute(null) == true)
                topCommand.Execute(null);
        }
        if (scrollViewer.VerticalOffset + scrollViewer.ViewportHeight >= scrollViewer.ExtentHeight && e.VerticalChange > 0)
        {
            ICommand bottomCommand = GetScrollBottomCommand(target);
            if (bottomCommand?.CanExecute(null) == true)
                bottomCommand.Execute(null);
        }
    }
    private static DependencyObject FindTargetWithCommands(ScrollViewer scrollViewer)
    {
        if (GetScrollTopCommand(scrollViewer) != null || GetScrollBottomCommand(scrollViewer) != null)
            return scrollViewer;

        DependencyObject parent = VisualTreeHelper.GetParent(scrollViewer);
        while (parent != null)
        {
            if (GetScrollTopCommand(parent) != null || GetScrollBottomCommand(parent) != null)
                return parent;
            parent = VisualTreeHelper.GetParent(parent);
        }
        return null!;
    }

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