using Melek.Domain;

namespace MtGBar.ViewModels
{
    public static class CardViewModelFactory
    {
        public static ICardViewModel GetCardViewModel(ICard card, IPrinting printing)
        {
            if (card.GetType() == typeof(FlipCard)) {
                return new FlipCardViewModel() {
                    Card = card as FlipCard,
                    Printing = printing as FlipPrinting
                };
            }
            else if(card.GetType() == typeof(SplitCard)) {
                return new SplitCardViewModel() {
                    Card = card as SplitCard,
                    Printing = printing as SplitPrinting
                };
            }
            else if (card.GetType() == typeof(TransformCard)) {
                return new TransformCardViewModel() {
                    Card = card as TransformCard,
                    Printing = printing as TransformPrinting
                };
            }
            else {
                return new CardViewModel() {
                    Card = card as Card,
                    Printing = printing as Printing
                };
            }
        }
    }
}