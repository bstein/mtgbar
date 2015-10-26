using System;
using System.Globalization;
using System.Windows.Data;
using Melek;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardPowerToughnessVisibilityConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            ICard typedValue = (value as ICard);
            if(typedValue.AllTypes != null) {
                foreach(CardType type in typedValue.AllTypes) {
                    if(type == CardType.Creature) {
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