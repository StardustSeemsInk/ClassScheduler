using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ClassScheduler.WPF.Converters;

public class HorizontalAlignmentReverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var align = (HorizontalAlignment)value;
        switch (align)
        {
            case HorizontalAlignment.Left:
                return HorizontalAlignment.Right;
            case HorizontalAlignment.Center:
                return HorizontalAlignment.Stretch;
            case HorizontalAlignment.Right:
                return HorizontalAlignment.Left;
            case HorizontalAlignment.Stretch:
                return HorizontalAlignment.Center;
        }

        return align;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
