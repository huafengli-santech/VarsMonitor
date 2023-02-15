using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MonitorApp.Converts
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class TagToVisbilityConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int c = System.Convert.ToInt32(parameter);

            if (value == null)
                throw new ArgumentNullException("value can not be null");

            int index = System.Convert.ToInt32(value);
            if (index != 0)
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