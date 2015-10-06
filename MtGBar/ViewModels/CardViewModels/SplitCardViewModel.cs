using Melek.Domain;

namespace MtGBar.ViewModels
{
    public class SplitCardViewModel : ICardViewModel
    {
        public SplitCard Card { get; set; }
        public SplitPrinting Printing { get; set; }
    }
}