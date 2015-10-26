using System.Windows.Input;
using System.Windows.Media.Imaging;
using Bazam.Wpf.UIHelpers;
using Bazam.Wpf.ViewModels;
using Melek;

namespace MtGBar.ViewModels
{
    public class TransformCardViewModel : ViewModelBase<TransformCardViewModel>, ICardViewModel
    {
        public TransformCard Card { get; set; }
        public TransformPrinting Printing { get; set; }

        private bool _IsTransformed = false;
        public bool IsTransformed
        {
            get { return _IsTransformed; }
            set { ChangeProperty(vm => vm.IsTransformed, value); }
        }

        private BitmapImage _NormalImage;
        public BitmapImage NormalImage
        {
            get { return _NormalImage; }
            set { ChangeProperty(vm => vm.NormalImage, value); }
        }

        public ICommand TransformCommand
        {
            get { return new RelayCommand(() => { this.IsTransformed = !this.IsTransformed; }); }
        }

        private BitmapImage _TransformedImage;
        public BitmapImage TransformedImage
        {
            get { return _TransformedImage; }
            set { ChangeProperty(vm => vm.TransformedImage, value); }
        }   
    }
}