using System.Windows.Media.Imaging;
using BazamWPF.ViewModels;
using Melek.Models;

namespace MtGBar.ViewModels
{
    public class CardViewModel : ViewModelBase
    {
        [RelatedProperty("FullSize")]
        private BitmapImage _FullSize;
        [RelatedProperty("Thumbnail")]
        private CroppedBitmap _Thumbnail;

        public Card Card { get; private set; }

        public BitmapImage FullSize
        {
            get { return _FullSize; }
            set { ChangeProperty<CardViewModel>(c => c.FullSize, value); }
        }

        public CroppedBitmap Thumbnail 
        {
            get { return _Thumbnail; }
            set { ChangeProperty<CardViewModel>(c => c.Thumbnail, value); }
        }

        public CardViewModel(Card card)
        {
            Card = card;
            FullSize = new BitmapImage();
            Thumbnail = new CroppedBitmap();
        }
    }
}
