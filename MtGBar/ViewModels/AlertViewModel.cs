using System;
using System.Windows.Media;
using Bazam.Wpf.ViewModels;

namespace MtGBar.ViewModels
{
    public class AlertViewModel : ViewModelBase<AlertViewModel>
    {
        #region Events
        public event EventHandler CloseRequested;
        #endregion
        
        private ImageSource _Background = null;
        public ImageSource Background
        {
            get { return _Background; }
            set { ChangeProperty(vm => vm.Background, value); }
        }
        
        private string _ContentSource = string.Empty;
        public string ContentSource
        {
            get { return _ContentSource; }
            set { ChangeProperty(vm => vm.ContentSource, value); }
        }
        
        private string _Message = string.Empty;
        public string Message
        {
            get { return _Message; }
            set { ChangeProperty(vm => vm.Message, value); }
        }
        
        private string _WindowSubTitle = string.Empty;
        public string WindowSubTitle
        {
            get { return _WindowSubTitle; }
            set { ChangeProperty(vm => vm.WindowSubTitle, value); }
        }
        
        private string _WindowTitle = string.Empty;
        public string WindowTitle 
        {
            get { return _WindowTitle; }
            set { ChangeProperty(vm => vm.WindowTitle, value); }
        }

        protected void RaiseCloseRequested()
        {
            if (CloseRequested != null) {
                CloseRequested(this, EventArgs.Empty);
            }
        }
    }
}
