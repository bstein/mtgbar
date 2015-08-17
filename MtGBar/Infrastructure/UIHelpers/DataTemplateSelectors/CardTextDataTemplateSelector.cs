using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Melek.Domain;

namespace MtGBar.Infrastructure.UIHelpers.DataTemplateSelectors
{
    public class CardTextDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LoyaltyCostTemplate { get; set; }
        public DataTemplate ManaCostTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item != null) {
                if (item.GetType() == typeof(CardCost) && (item as CardCost).Type != CardCostType.OTHER) {
                    return ManaCostTemplate;
                }
                else if (item.GetType() == typeof(string) && Regex.IsMatch(item.ToString(), "([-\\+]?[1-9][0-9]*|0|X):")) {
                    return LoyaltyCostTemplate;
                }
            }
            return TextTemplate;
        }
    }
}