using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class EnumerableStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;

            IEnumerable typedValue = (IEnumerable)value;
            bool typedParameter = (parameter == null ? false : bool.Parse(parameter.ToString()));

            if (typedValue != null) {
                foreach (object item in typedValue) {
                    retVal += item.ToString() + " ";
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