using System;
using System.Globalization;
using System.Windows.Data;
using Melek.Models;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardPowerToughnessVisibilityConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Card typedValue = (value as Card);
            if(typedValue.CardTypes != null) {
                foreach(CardType type in typedValue.CardTypes) {
                    if(type == CardType.CREATURE) {
                        return true;
                    }
                }
            }
            return false;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
