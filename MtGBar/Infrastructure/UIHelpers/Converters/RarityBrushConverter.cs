using System;
using System.Globalization;
using System.Windows.Data;
using MtGBar.Infrastructure.DataNinjitsu.Models;
using Melek;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class RarityBrushConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null) {
                switch ((value as ICard).Printings[0].Rarity) {
                    case CardRarity.Common:
                        return App.Current.FindResource("CommonBrush");
                    case CardRarity.Uncommon:
                        return App.Current.FindResource("UncommonBrush");
                    case CardRarity.Rare:
                        return App.Current.FindResource("RareBrush");
                    case CardRarity.MythicRare:
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
