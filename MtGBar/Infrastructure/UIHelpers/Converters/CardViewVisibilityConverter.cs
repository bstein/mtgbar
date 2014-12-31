using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using MtGBar.Infrastructure.DataNinjitsu.Models;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardViewVisibilityConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
