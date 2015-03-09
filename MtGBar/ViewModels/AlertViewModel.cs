using System;
using System.Windows.Media;
using BazamWPF.ViewModels;

namespace MtGBar.ViewModels
{
    public class AlertViewModel : ViewModelBase
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

        [RelatedProperty("ContentSource")]
        private string _ContentSource = string.Empty;
        public string ContentSource
        {
            get { return _ContentSource; }
            set { ChangeProperty<AlertViewModel>(vm => vm.ContentSource, value); }
        }

        [RelatedProperty("Message")]
        private string _Message = string.Empty;
        public string Message
        {
            get { return _Message; }
            set { ChangeProperty<WelcomeViewModel>(vm => vm.Message, value); }
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
