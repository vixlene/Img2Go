using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Img2Go.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isNull = value == null;
            var inverse = parameter?.ToString() == "Inverse";
            
            if (inverse)
                return isNull ? Visibility.Collapsed : Visibility.Visible;
            return isNull ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

