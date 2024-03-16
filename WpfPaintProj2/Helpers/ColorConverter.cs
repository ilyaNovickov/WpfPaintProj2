using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfPaintProj2.Helpers
{
    [ValueConversion(typeof(Brush), typeof(Color?))]
    internal class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }
    }

    [ValueConversion(typeof(Color?), typeof(Brush))]
    internal class BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color?)
                return new SolidColorBrush(((Color?)value).Value);
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color;
        }
    }
}
