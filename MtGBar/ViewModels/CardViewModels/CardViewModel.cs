using Melek.Domain;

namespace MtGBar.ViewModels
{
    public class CardViewModel : ICardViewModel
    {
        public Card Card { get; set; }
        public Printing Printing { get; set; }
    }
}