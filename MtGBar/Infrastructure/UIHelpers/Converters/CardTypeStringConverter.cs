using System;
using System.Globalization;
using System.Windows.Data;
using Melek.Domain;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            bool typedParameter = (parameter == null ? false : bool.Parse(parameter.ToString()));

            if (value != null) {
                ICard typedValue = (value as ICard);
                foreach (CardType type in typedValue.AllTypes) {
                    retVal += type.ToString() + " ";
                }
            }

            if (typedParameter) retVal = retVal.ToUpper();

            return retVal.Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}