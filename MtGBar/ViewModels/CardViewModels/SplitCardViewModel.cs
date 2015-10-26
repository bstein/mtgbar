using System.Windows.Media.Imaging;
using Bazam.Wpf.ViewModels;
using Melek;

namespace MtGBar.ViewModels
{
    public class SplitCardViewModel : ViewModelBase<SplitCardViewModel>, ICardViewModel
    {
        public SplitCard Card { get; set; }
        public SplitPrinting Printing { get; set; }

        private BitmapImage _CardImage = null;
        public BitmapImage CardImage
        {
            get { return _CardImage; }
            set { ChangeProperty(vm => vm.CardImage, value); }
        }
    }
}