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
        return align switch
        {
            HorizontalAlignment.Left => HorizontalAlignment.Right,
            HorizontalAlignment.Center => HorizontalAlignment.Stretch,
            HorizontalAlignment.Right => HorizontalAlignment.Left,
            HorizontalAlignment.Stretch => HorizontalAlignment.Center,
            _ => (object)align,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
