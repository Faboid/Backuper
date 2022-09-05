using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Backuper.UI.WPF.Converters; 
public class AddTwoLeftMarginConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var thickness = (Thickness)value;
        thickness.Left += 2;
        return thickness;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        var thickness = (Thickness)value;
        thickness.Left -= 2;
        return thickness;
    }
}
