using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardTextVisibilityConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null) {
                CardTextConverter textConverter = new CardTextConverter();
                string cardText = textConverter.Convert(value, typeof(string), null, culture).ToString();
                return (string.IsNullOrEmpty(cardText) ? Visibility.Collapsed : Visibility.Visible);
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
