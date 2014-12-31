using System.Windows;
using System.Windows.Media.Imaging;
using BazamWPF.ViewModels;
using Melek.Models;

namespace MtGBar.ViewModels
{
    public class CardViewModel : ViewModelBase
    {
        private BitmapImage _FullSize;
        private CroppedBitmap _Thumbnail;

        public Card Card { get; private set; }

        public BitmapImage FullSize
        {
            get { return _FullSize; }
            set
            {
                if (_FullSize != value) {
                    _FullSize = value;
                    OnPropertyChanged("FullSize");
                }
            }
        }

        public CroppedBitmap Thumbnail 
        {
            get { return _Thumbnail; }
            set
            {
                if (_Thumbnail != value) {
                    _Thumbnail = value;
                    OnPropertyChanged("Thumbnail");
                }
            }
        }

        public CardViewModel(Card card)
        {
            Card = card;
            FullSize = new BitmapImage();
            Thumbnail = new CroppedBitmap();
        }
    }
}
