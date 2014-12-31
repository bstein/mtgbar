using System;
using System.Globalization;
using System.Windows.Data;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class DateTimeStringConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null) {
                DateTime typedValue = (DateTime)value;
                if (parameter != null && Boolean.Parse(parameter.ToString())) {
                    return typedValue.ToShortTimeString();
                }
                return typedValue.ToShortDateString();
            }
            return string.Empty;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}