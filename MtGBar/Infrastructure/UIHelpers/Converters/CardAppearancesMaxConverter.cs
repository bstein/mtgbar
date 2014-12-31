using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Melek.Models;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardAppearancesMaxConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (value as List<CardAppearance>).OrderByDescending(a => a.Set.Date).Take(5).ToArray();
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
