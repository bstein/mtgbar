using System;
using System.Globalization;
using System.Windows.Data;
using MtGBar.Infrastructure.DataNinjitsu.Models.TweetWords;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class TwitterMentionUrlConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return "https://twitter.com/" + (value as Mention).Text.Replace("@", string.Empty).Replace(":", string.Empty);
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
