using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class LoyaltyCostIconConverter : IValueConverter
    {
        private const int SOFT_HYPHEN_CODE = 8722;

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null) {
                // defaulting to neg 1 handles the ashiok case. i don't super love it, i'm just lazy right now
                int loyalty = -1;

                string typedValue = value.ToString();
                typedValue = typedValue.Replace(":", string.Empty);
                Int32.TryParse(typedValue, out loyalty);

                string fileName = "pack://application:,,,/Assets/loyalty/loyalty-";

                if (loyalty > 0) {
                    fileName += "up.png";
                }
                else if (loyalty == 0) {
                    fileName += "zero.png";
                }
                else {
                    fileName += "down.png";
                }

                Uri uri = new Uri(fileName);
                BitmapImage bmp = new BitmapImage(uri);
                return new BitmapImage(uri);
            }

            return null;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
