using SpoofFileParser.FileMetadata;
using SpoofMess.Models;
using System.Windows;
using System.Windows.Controls;

namespace SpoofMess.ViewElements;

public class ImagePanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        double width = Math.Min(availableSize.Width, 300),
            height = 0, currentHeight;
        int index = InternalChildren.Count % 3;

        List<UIElement> list = [.. InternalChildren.Cast<UIElement>()];
        if (index != 0)
        {
            height += GetHeight2(list[..index], width);
            foreach (UIElement element in list[..index])
                element.Measure(new(width, height));
        }
        for (; index < InternalChildren.Count; index += 3)
        {
            currentHeight = GetHeight2(list.Slice(index, 3), width);
            height += currentHeight;
            foreach (UIElement element in list.Slice(index, 3))
                element.Measure(new(width, currentHeight));
        }
        return new Size(width, height);
    }

    private static double GetHeight2(List<UIElement> childrens, double width)
    {
        double height = 0, ratio; 
        if (childrens.Count == 1)
        {
            var element = childrens[0];
            if (element is FrameworkElement { DataContext: FileObject { Metadata: ImageMetadata m } })
            {
                ratio = (double)m.Width / m.Height;

                if (ratio > 1.5) return width / 2;
                if (ratio < 0.5) return 500;

                return width / ratio;
            }
        }
        foreach (UIElement element in childrens)
            if (element is FrameworkElement { DataContext: FileObject { Metadata: ImageMetadata metadata } })
            {
                ratio = metadata.Height > 0 ?  (double)metadata.Width / metadata.Height : 1;
                height += ratio;
            }    

        return height > 0 ? Math.Min(width / height, 500) : 0;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int count = InternalChildren.Count;
        if (count == 0)
            return finalSize;
        int firstItems = count % 3,
            rowCount = count / 3,
            childIndex = 0;

        Rect rect;
        List<UIElement> list = [.. InternalChildren.Cast<UIElement>()];
        double height = GetHeight2(list[..firstItems], finalSize.Width),
            itemWidth,
            currentX = 0,
            currentY = 0;
        for (int r = 0; r < firstItems; r++)
        {
            if (childIndex >= count)
                return finalSize;
            if (list[childIndex] is FrameworkElement { DataContext: FileObject { Metadata: ImageMetadata metadata } })
            {
                itemWidth = (double)metadata.Width / metadata.Height * height;
                rect = new(currentX, 0, itemWidth, height);
                currentX += itemWidth;
                InternalChildren[childIndex++].Arrange(rect);
            }
        }
        for (int r = 0; r < rowCount; r++)
        {
            currentX = 0;
            currentY += height;
            height = GetHeight2(list.Slice(firstItems + r * 3, 3), finalSize.Width);
            for (int c = 0; c < 3; c++)
            {
                if (childIndex >= count)
                    return finalSize;
                if (list[childIndex] is FrameworkElement { DataContext: FileObject { Metadata: ImageMetadata metadata } })
                {
                    itemWidth = (double)metadata.Width / metadata.Height * height;
                    rect = new(currentX, currentY, itemWidth, height);
                    currentX += itemWidth;
                    InternalChildren[childIndex++].Arrange(rect);
                }
                else
                    childIndex++;
            }
        }
        return finalSize;
    }
}
