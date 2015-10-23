using System.Windows.Input;
using System.Windows.Media.Imaging;
using Bazam.Wpf.UIHelpers;
using Bazam.Wpf.ViewModels;
using Melek.Domain;

namespace MtGBar.ViewModels
{
    public class FlipCardViewModel : ViewModelBase<FlipCardViewModel>, ICardViewModel
    {        
        public FlipCard Card { get; set; }
        public BitmapImage CardImage { get; set; }
        public FlipPrinting Printing { get; set; }

        private bool _IsFlipped = false;
        public bool IsFlipped
        {
            get { return _IsFlipped; }
            set { ChangeProperty(vm => vm.IsFlipped, value); }
        }

        public ICommand FlipCommand
        {
            get { return new RelayCommand(() => { this.IsFlipped = !this.IsFlipped; }); }
        }
    }
}