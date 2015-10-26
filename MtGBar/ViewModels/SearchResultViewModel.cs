using System.Windows.Media.Imaging;
using Bazam.Wpf.ViewModels;
using Melek;

namespace MtGBar.ViewModels
{
    public class SearchResultViewModel : ViewModelBase<SearchResultViewModel>
    {
        private ICard _Card;
        private BitmapImage _FullSize;
        private CroppedBitmap _Thumbnail;

        public ICard Card
        {
            get { return _Card; }
            set { ChangeProperty(c => c.Card, value); }
        }

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
    }
}