using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ModPlusAPI.Windows;
using Brush = System.Drawing.Brush;

namespace whshScheduleLookup.Views.Converters
{
    [ValueConversion(typeof(int?), typeof(Brush))]
    public class IntToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (int)value < 1)
            {
                return new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Black); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ModPlusAPI.Windows.MessageBox.Show("Back converting failed", MessageBoxIcon.Close);
            
            return 0;
        }
    }
}
