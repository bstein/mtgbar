using System;
using System.Globalization;
using System.Windows.Data;
using Melek.Domain;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardRarityStringConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null) {
                switch ((value as Card).Printings[0].Rarity) {
                    case CardRarity.Common:
                        return "COMMON";
                    case CardRarity.Uncommon:
                        return "UNCOMMON";
                    case CardRarity.Rare:
                        return "RARE";
                    case CardRarity.MythicRare:
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
