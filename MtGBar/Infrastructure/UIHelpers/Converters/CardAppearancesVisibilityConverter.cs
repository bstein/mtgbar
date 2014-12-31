using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Melek.Models;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardAppearancesVisibilityConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            List<CardAppearance> appearances = value as List<CardAppearance>;
            return appearances.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}