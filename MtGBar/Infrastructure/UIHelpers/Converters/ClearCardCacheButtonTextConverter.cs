using System;
using System.Globalization;
using System.Windows.Data;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class ClearCardCacheButtonTextConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return "delete saved images (" + value.ToString() + ")";
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
