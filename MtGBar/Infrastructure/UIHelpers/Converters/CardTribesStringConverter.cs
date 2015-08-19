using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardTribesStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            
            if (value != null) {
                IReadOnlyList<string> typedValue = (value as IReadOnlyList<string>);
                foreach (string tribe in typedValue) {
                    retVal += tribe.ToString() + " ";
                }
            }
            
            return retVal.Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}