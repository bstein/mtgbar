using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class LoyaltyCostMarginConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null) {
                int loyalty = -1;
                Int32.TryParse(value.ToString().Replace(":", string.Empty), out loyalty);

                if (loyalty > 0) {
                    return new Thickness(0, -1, 0, 0);
                }
                else if (loyalty == 0) {
                    return new Thickness(3, -2, 0, 0);
                }
                else {
                    return new Thickness(0, -3, 0, 0);
                }
            }

            return new Thickness(0, -4, 0, 0);
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}