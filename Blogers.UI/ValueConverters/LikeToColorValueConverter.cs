using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Blogers.UI.ValueConverters
{
    class LikeToColorValueConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if((int)value < 10)
            {
                return Colors.DarkRed;
            }
            return Colors.WhiteSmoke;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
