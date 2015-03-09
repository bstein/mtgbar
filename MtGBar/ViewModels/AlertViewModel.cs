using System;
using System.Windows.Media;
using BazamWPF.ViewModels;

namespace MtGBar.ViewModels
{
    public abstract class AlertViewModel : ViewModelBase
    {
        #region Events
        public event EventHandler CloseRequested;
        #endregion

        [RelatedProperty("Background")]
        private ImageSource _Background = null;
        public ImageSource Background
        {
            get { return _Background; }
            set { ChangeProperty<AlertViewModel>(vm => vm.Background, value); }
        }

        [RelatedProperty("WindowSubTitle")]
        private string _WindowSubTitle = string.Empty;
        public string WindowSubTitle
        {
            get { return _WindowSubTitle; }
            set { ChangeProperty<AlertViewModel>(vm => vm.WindowSubTitle, value); }
        }

        [RelatedProperty("WindowTitle")]
        private string _WindowTitle = string.Empty;
        public string WindowTitle 
        {
            get { return _WindowTitle; }
            set { ChangeProperty<AlertViewModel>(vm => vm.WindowTitle, value); }
        }

        protected void RaiseCloseRequested()
        {
            if (CloseRequested != null) {
                CloseRequested(this, EventArgs.Empty);
            }
        }
    }
}
