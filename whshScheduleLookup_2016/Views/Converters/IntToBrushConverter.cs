namespace whshScheduleLookup.Views.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using ModPlusAPI.Windows;

    [ValueConversion(typeof(int?), typeof(Brush))]
    public class IntToBrushConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (int)value < 1)
            {
                return new SolidColorBrush(Colors.Red);
            }

            return new SolidColorBrush(Colors.Black); 
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ModPlusAPI.Windows.MessageBox.Show("Back converting failed", MessageBoxIcon.Close);
            
            return 0;
        }
    }
}
