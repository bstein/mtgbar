using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Melek.Models;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardFirstPrintingConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            IOrderedEnumerable<CardPrinting> printings = (value as List<CardPrinting>).OrderByDescending(a => a.Set.Date);
            CardPrinting first = printings.FirstOrDefault(a => a.Set.IsPromo == false);
            if (first == null) {
                first = printings.FirstOrDefault();
            }
            return first;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
