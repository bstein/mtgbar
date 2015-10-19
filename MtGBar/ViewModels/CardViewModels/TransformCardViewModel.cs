using System.Windows.Media.Imaging;
using Bazam.Wpf.ViewModels;
using Melek.Domain;

namespace MtGBar.ViewModels
{
    public class TransformCardViewModel : ViewModelBase<TransformCardViewModel>, ICardViewModel
    {
        public TransformCard Card { get; set; }
        public TransformPrinting Printing { get; set; }

        private BitmapImage _NormalImage;
        public BitmapImage NormalImage
        {
            get { return _NormalImage; }
            set { ChangeProperty(vm => vm.NormalImage, value); }
        }

        private BitmapImage _TransformedImage;
        public BitmapImage TransformedImage
        {
            get { return _TransformedImage; }
            set { ChangeProperty(vm => vm.TransformedImage, value); }
        }
    }
}