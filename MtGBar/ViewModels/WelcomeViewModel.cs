using System;
using System.Windows.Media.Imaging;
using BazamWPF.ViewModels;
using MtGBar.Infrastructure;

namespace MtGBar.ViewModels
{
    public class WelcomeViewModel : AlertViewModel
    {
        #region Properties
        [RelatedProperty("IsLoading")]
        private bool _IsLoading = true;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { ChangeProperty<WelcomeViewModel>(vm => vm.IsLoading, value); }
        }

        [RelatedProperty("ShowWelcomeScreen")]
        private bool _ShowWelcomeScreen = true;
        public bool ShowWelcomeScreen
        {
            get { return _ShowWelcomeScreen; }
            set 
            { 
                ChangeProperty<WelcomeViewModel>(vm => vm.ShowWelcomeScreen, value);
                AppState.Instance.Settings.ShowWelcomeScreen = value;
                AppState.Instance.Settings.Save();

                if (!value) {
                    RaiseCloseRequested();
                }
            }
        }
        #endregion

        public WelcomeViewModel()
        {
            Background = new BitmapImage(new Uri("pack://application:,,,/Assets/welcome-bg.jpg"));
            ContentSource = "Views/AlertViews/WelcomeView.xaml";
            ShowWelcomeScreen = AppState.Instance.Settings.ShowWelcomeScreen;
            WindowSubTitle = "\"EITHER I KNOW JUST THE SPELL I NEED, OR I'M ABOUT TO.\"";
            WindowTitle = "Welcome to MtGBar";

            AppState.Instance.MelekDataStore.DataLoaded += (muchData, veryWow) => { IsLoading = false; };

            string welcome = "Alrighty! It's time.\n\n";
            if (AppState.Instance.Settings.Hotkey != null) {
                welcome += "Hit " + AppState.Instance.Settings.Hotkey.ToString() + " to get started!";
            }
            else {
                welcome += "Looks like you don't have a hotkey set up right now. Right-click on the System Tray icon and choose Settings to pick one, or just choose Browse to party without a hotkey. But that's way less fun.";
            }

            Message = welcome;
        }
    }
}