using SpoofMess.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SpoofMess.ViewElements;

public class ImagePanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        double width = Math.Min(availableSize.Width, 300);
        double height = 0;
        int index = InternalChildren.Count % 3;
        List<UIElement> list = InternalChildren.Cast<UIElement>().ToList();
        if (index != 0)
            height += GetHeight(list[..index], width);
        for (; index < InternalChildren.Count; index += 3)
            height += GetHeight(list.Slice(index, 3), width);

        return new Size(width, height);
    }

    private double GetHeight(List<UIElement> childrens, double width)
    {
        double height = 0;
        BitmapDecoder decoder;
        foreach (UIElement element in childrens)
        {
            if (element is FrameworkElement fe && fe.DataContext is FileObject file && file.Path is not null)
            {
                decoder = BitmapDecoder.Create(new Uri(file.Path), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                height += (decoder.Frames[0].Width / decoder.Frames[0].Height);
            }
        }
        return width / height;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int count = InternalChildren.Count;
        if (count == 0)
            return finalSize;
        int firstItems = count % 3;
        int rowCount = count / 3;
        double rowHeight = finalSize.Height / (rowCount + (firstItems > 0 ? 1 : 0));
        int childIndex = 0;
        double childWidth;

        Rect rect;
        List<UIElement> list = InternalChildren.Cast<UIElement>().ToList();
        BitmapDecoder decoder;
        double height = GetHeight(list[..firstItems], finalSize.Width);
        double itemWidth, currentX = 0, currentY = 0;
        for (int r = 0; r < firstItems; r++)
        {
            if (childIndex >= count)
                return finalSize;
            if (list[r] is FrameworkElement fe && fe.DataContext is FileObject file && file.Path is not null)
            {
                decoder = BitmapDecoder.Create(new Uri(file.Path), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                itemWidth = decoder.Frames[0].Width / decoder.Frames[0].Height * height;
                rect = new(currentX, 0, itemWidth, height);
                currentX += itemWidth;
                InternalChildren[childIndex++].Arrange(rect);
            }
        }
        for (int r = 0; r < rowCount; r++)
        {
            currentX = 0;
            currentY += height;
            childWidth = finalSize.Width / 3;
            height = GetHeight(list.Slice(firstItems + r * 3, 3), finalSize.Width);
            for (int c = 0; c < 3; c++)
            {
                if (childIndex >= count)
                    return finalSize;
                if (list[childIndex] is FrameworkElement fe && fe.DataContext is FileObject file && file.Path is not null)
                {
                    decoder = BitmapDecoder.Create(new Uri(file.Path), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                    itemWidth = decoder.Frames[0].Width / decoder.Frames[0].Height * height;
                    rect = new(currentX, currentY, itemWidth, height);
                    currentX += itemWidth;
                    InternalChildren[childIndex++].Arrange(rect);
                }
            }
        }
        return finalSize;
    }
}
