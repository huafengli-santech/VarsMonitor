using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace MonitorApp.Converts
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class BoolToVisBilityConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int c = System.Convert.ToInt32(parameter);

            if (value == null)
                throw new ArgumentNullException("value can not be null");

            Boolean index = System.Convert.ToBoolean(value);
            if (index)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}