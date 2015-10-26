using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Melek;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardCostConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null) {
                CardCost cost = (value as CardCost);
                Uri uri = null;

                if (cost.Type == CardCostType._) {
                    uri = new Uri("pack://application:,,,/Assets/manaSymbols/" + cost.Quantity.ToString().ToLower() + ".png");
                }
                else {
                    uri = new Uri("pack://application:,,,/Assets/manaSymbols/" + cost.ToString().ToLower() + ".png");
                }

                BitmapImage bmp = new BitmapImage(uri);
                return bmp;
            }
            
            return null;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}