using Melek.Domain;

namespace MtGBar.ViewModels
{
    public class FlipCardViewModel : ICardViewModel
    {
        public FlipCard Card { get; set; }
        public FlipPrinting Printing { get; set; }
    }
}