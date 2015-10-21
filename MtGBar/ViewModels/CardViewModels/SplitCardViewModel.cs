using System.Windows.Media.Imaging;
using Melek.Domain;

namespace MtGBar.ViewModels
{
    public class SplitCardViewModel : ICardViewModel
    {
        public SplitCard Card { get; set; }
        public BitmapImage CardImage { get; set; }
        public SplitPrinting Printing { get; set; }
    }
}