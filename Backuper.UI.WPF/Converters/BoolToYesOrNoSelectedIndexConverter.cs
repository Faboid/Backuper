using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Backuper.UI.WPF.Converters;

public class BoolToYesOrNoSelectedIndexConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

        if(value is bool boolean) {
            return boolean ? 1 : 0;
        }

        return DependencyProperty.UnsetValue;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

        if(value is int integer) {

            if(integer == 1) {
                return true;
            }

            if(integer == 0) {
                return false;
            }

        }

        return DependencyProperty.UnsetValue;

    }
}
