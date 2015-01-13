using System;
using System.Globalization;
using System.Windows.Data;
using MtGBar.ViewModels;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    class TweetViewModelAuthorUrlConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString())) {
                return "https://twitter.com/" + value.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
