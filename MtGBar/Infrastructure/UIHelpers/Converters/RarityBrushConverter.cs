using System;
using System.Globalization;
using System.Windows.Data;
using MtGBar.Infrastructure.DataNinjitsu.Models;
using Melek.Models;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class RarityBrushConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null) {
                switch ((value as Card).Printings[0].Rarity) {
                    case CardRarity.C:
                        return App.Current.FindResource("CommonBrush");
                    case CardRarity.U:
                        return App.Current.FindResource("UncommonBrush");
                    case CardRarity.R:
                        return App.Current.FindResource("RareBrush");
                    case CardRarity.M:
                        return App.Current.FindResource("MythicBrush");
                    default:
                        return App.Current.FindResource("ForegroundBrush");

                }
            }
            return App.Current.FindResource("ForegroundBrush");
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
