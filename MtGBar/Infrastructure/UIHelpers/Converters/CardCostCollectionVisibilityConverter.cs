using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Melek;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardCostCollectionVisibilityConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null && (value as CardCostCollection).Count() > 0) {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}