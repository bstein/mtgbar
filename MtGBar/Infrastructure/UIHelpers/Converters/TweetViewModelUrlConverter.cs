using System;
using System.Globalization;
using System.Windows.Data;
using MtGBar.ViewModels;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class TweetViewModelUrlConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            TweetViewModel typedVal = (TweetViewModel)value;
            return string.Format("https://twitter.com/{0}/status/{1}", typedVal.AuthorTwitterName, typedVal.TweetID);
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}