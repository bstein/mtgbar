using System;
using System.Globalization;
using System.Windows.Data;
using Melek.Domain;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardTypeStringConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            if (value != null) {
                ICard typedValue = (value as ICard);
                foreach (CardType type in typedValue.AllTypes) {
                    retVal += type.ToString() + " ";
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