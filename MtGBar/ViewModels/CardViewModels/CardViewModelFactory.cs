using System.Threading.Tasks;
using Bazam.Wpf.UIHelpers;
using Melek.Client.DataStore;
using Melek.Domain;

namespace MtGBar.ViewModels
{
    public static class CardViewModelFactory
    {
        public static async Task<ICardViewModel> GetCardViewModel(ICard card, IPrinting printing, MelekClient client)
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
                    CardImage = await ImageFactory.FromUri(await client.GetImageUri(printing)),
                    Printing = printing as SplitPrinting
                };
            }
            else if (card.GetType() == typeof(TransformCard)) {
                return new TransformCardViewModel() {
                    Card = card as TransformCard,
                    Printing = printing as TransformPrinting,
                    NormalImage = await ImageFactory.FromUri(await client.GetImageUri(printing)),
                    TransformedImage = await ImageFactory.FromUri(await client.GetImageUri(printing as TransformPrinting, false))
                };
            }
            else {
                return new CardViewModel() {
                    Card = card as Card,
                    CardImage = await ImageFactory.FromUri(await client.GetImageUri(printing)),
                    Printing = printing as Printing
                };
            }
        }
    }
}