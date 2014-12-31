using System;
using System.Globalization;
using System.Windows.Data;
using Melek.Models;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardTypeStringConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            if (value != null) {
                Card typedValue = (value as Card);
                if (typedValue.CardTypes != null) {
                    foreach (CardType type in typedValue.CardTypes) {
                        retVal += type.ToString() + " ";
                    }
                }
            }

            return retVal.Trim();
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}