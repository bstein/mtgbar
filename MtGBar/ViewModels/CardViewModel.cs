using System.Windows.Media.Imaging;
using Bazam.Wpf.ViewModels;
using Melek.Domain;

namespace MtGBar.ViewModels
{
    public class CardViewModel : ViewModelBase<CardViewModel>
    {
        private BitmapImage _FullSize;
        private CroppedBitmap _Thumbnail;

        public ICard Card { get; private set; }

        public BitmapImage FullSize
        {
            get { return _FullSize; }
            set { ChangeProperty(c => c.FullSize, value); }
        }

        public CroppedBitmap Thumbnail 
        {
            get { return _Thumbnail; }
            set { ChangeProperty(c => c.Thumbnail, value); }
        }

        public CardViewModel(ICard card)
        {
            Card = card;
            FullSize = new BitmapImage();
            Thumbnail = new CroppedBitmap();
        }
    }
}
