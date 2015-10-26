using System.Windows.Media.Imaging;
using Bazam.Wpf.ViewModels;
using Melek;

namespace MtGBar.ViewModels
{
    public class CardViewModel : ViewModelBase<CardViewModel>, ICardViewModel
    {
        public Card Card { get; set; }
        public Printing Printing { get; set; }

        private BitmapImage _CardImage = null;
        public BitmapImage CardImage
        {
            get { return _CardImage; }
            set { ChangeProperty(vm => vm.CardImage, value); }
        }
    }
}