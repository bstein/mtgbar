using System;
using System.Globalization;
using System.Windows.Data;
using MtGBar.Infrastructure.DataNinjitsu.Models.TweetWords;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class TwitterHashtagUrlConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return "https://twitter.com/hashtag/" + (value as Hashtag).Text.Replace("#", "");
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}