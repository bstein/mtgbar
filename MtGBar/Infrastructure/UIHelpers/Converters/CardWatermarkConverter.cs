using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using MtGBar.Infrastructure.Utilities;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardWatermarkConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString())) {
                try {
                    Uri uri = new Uri("pack://application:,,,/Assets/watermarks/" + value.ToString().ToLower() + ".png");
                    BitmapImage bmp = new BitmapImage(uri);
                    return new BitmapImage(uri);
                }
                catch (Exception) {
                    AppState.Instance.LoggingNinja.LogMessage("Need a watermark for " + value.ToString());
                }
            }
            return null;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}