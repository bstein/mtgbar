using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Melek;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardFirstPrintingConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            IOrderedEnumerable<IPrinting> printings = (value as IList<IPrinting>).OrderByDescending(a => a.Set.Date);
            IPrinting first = printings.FirstOrDefault(a => a.Set.IsPromo == false);
            if (first == null) {
                first = printings.FirstOrDefault();
            }
            return first;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}