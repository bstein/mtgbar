using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Melek.Domain;
using MtGBar.ViewModels;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class ResultsDelimiterVisibilityConverter : IMultiValueConverter
    {
        public Object Convert(Object[] value, Type targetType, Object parameter, CultureInfo culture)
        {
            if(value != null && value[0] != null) {
                IEnumerable<CardViewModel> cardMatches = value[0] as IEnumerable<CardViewModel>;
                Card selectedCard = value[1] as Card;

                return cardMatches.Count() > 0 && selectedCard == null ? Visibility.Visible : Visibility.Collapsed;
            }
            if (value != null) {
                
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
