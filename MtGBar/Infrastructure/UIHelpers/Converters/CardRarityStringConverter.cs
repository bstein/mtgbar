using System;
using System.Globalization;
using System.Windows.Data;
using Melek.Models;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardRarityStringConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null) {
                switch ((value as Card).Printings[0].Rarity) {
                    case CardRarity.C:
                        return "COMMON";
                    case CardRarity.U:
                        return "UNCOMMON";
                    case CardRarity.R:
                        return "RARE";
                    case CardRarity.M:
                        return "MYTHIC RARE";
                    default:
                        return "COMMON";
                }
            }
            return "COMMON";
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
