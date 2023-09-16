﻿using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace ClassScheduler.WPF.Converters;

public class RectConverter : IMultiValueConverter
{
    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return new Rect(0, 0, (double)values[0], (double)values[1]);
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}