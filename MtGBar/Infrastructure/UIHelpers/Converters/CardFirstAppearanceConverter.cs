using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Melek.Models;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardFirstAppearanceConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (value as List<CardAppearance>).OrderByDescending(a => a.Set.Date).FirstOrDefault();
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
