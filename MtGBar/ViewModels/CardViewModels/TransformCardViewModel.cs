using Melek.Domain;

namespace MtGBar.ViewModels
{
    public class TransformCardViewModel : ICardViewModel
    {
        public TransformCard Card { get; set; }
        public TransformPrinting Printing { get; set; }
    }
}