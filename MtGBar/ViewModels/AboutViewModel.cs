using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using Bazam.APIs;
using Bazam.KeyAdept;
using Bazam.Modules;
using BazamWPF.ViewModels;
using Melek.Models;
using MtGBar.Infrastructure;
using MtGBar.Infrastructure.DataNinjitsu.Models;
using MtGBar.Infrastructure.UIHelpers.Commands;
using MtGBar.Infrastructure.Utilities;

namespace MtGBar.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private string _CardsDirectorySize = string.Empty;
        private ICommand _ClearAppDataCacheCommand = new ClearAppDataCacheCommand();
        private ICommand _ClearCardCacheCommand = new ClearCardCacheCommand();
        private DisplayViewModel[] _DisplayViewModels;
        private HotkeyDescription _Hotkey { get; set; }
        private string _HotkeyString { get; set; }
        private IEnumerable<Package> _Packages { get; set; }
        private DisplayViewModel _SelectedDisplay = null;
        public IEnumerable<TweetViewModel> _Tweets = null;

        public AboutViewModel()
        {
            // set hotkey
            _Hotkey = AppState.Instance.Settings.Hotkey;

            // get packages
            GetPackages();

            // find out about the card cache size
            QueryCardCacheSize();

            // set version
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            try {
                version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception) { }
            VersionString = "v " + version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Revision.ToString();

            // listen in case the packages get updated so we can requery things
            AppState.Instance.MelekDataStore.PackagesUpdated += (newPackages) => {
                GetPackages();
            };

            // query displays
            List<DisplayViewModel> displayVMs = new List<DisplayViewModel>();
            displayVMs.Add(new DisplayViewModel(null));

            Screen[] displays = MonitorLizard.GetDisplays();
            
            for (int i = 0; i < displays.Length; i++) {
                DisplayViewModel vm = new DisplayViewModel(displays[i], i);
                displayVMs.Add(vm);

                if (i == AppState.Instance.Settings.DisplayIndex) {
                    SelectedDisplay = vm;
                }
            }
            _DisplayViewModels = displayVMs.ToArray();
            if (SelectedDisplay == null) SelectedDisplay = displayVMs[0];

            // get tweets
            GetTweets();
        }

        public string CardsDirectorySize
        {
            get { return _CardsDirectorySize; }
            set
            {
                if (_CardsDirectorySize != value) {
                    _CardsDirectorySize = value;
                    OnPropertyChanged("CardsDirectorySize");
                }
            }
        }

        public ICommand ClearAppDatacacheCommand
        {
            get { return _ClearAppDataCacheCommand; }
        }

        public ICommand ClearCardCacheCommand
        {
            get { return _ClearCardCacheCommand; }
        }

        public bool DismissOnFocusLoss
        {
            get { return AppState.Instance.Settings.DismissOnFocusLoss; }
            set
            {
                if (AppState.Instance.Settings.DismissOnFocusLoss != value) {
                    AppState.Instance.Settings.DismissOnFocusLoss = value;
                    AppState.Instance.Settings.Save();
                    OnPropertyChanged("DismissOnFocusLoss");
                }
            }
        }

        public DisplayViewModel[] Displays
        {
            get { return _DisplayViewModels; }
        }

        public string DonateUrl
        {
            get { return AppConstants.PAYPAL_DONATE_URL; }
        }

        public IEnumerable<Package> Packages
        {
            get { return _Packages; }
            set
            {
                _Packages = value;
                OnPropertyChanged("Packages");
            }
        }

        public HotkeyDescription Hotkey
        {
            get { return _Hotkey; }
            set
            {
                if (_Hotkey != value) {
                    _Hotkey = value;
                    HotkeyRegistrar registrar = AppState.Instance.HotkeyRegistrar;

                    if (AppState.Instance.Settings.Hotkey != null) {
                        registrar.UnregisterAllHotkeys();
                    }

                    bool commitToSettings = true;
                    if (value != null) {
                        if (registrar.IsHotkeyAvailable(value.ToHotkey())) {
                            AppState.Instance.RegisterHotkey(value);
                        }
                        else {
                            commitToSettings = false;
                            _Hotkey = null;
                            TalkAtcha.TalkAtEm("Sorry.", value.ToString() + " isn't available for use with " + AppConstants.APPNAME + ". This is usually because some other software is using that hotkey. Try another! CTRL + SPACE, CTRL + ALT + SPACE, and CTRL + J are usually good bets.");
                        }
                    }

                    if (commitToSettings) {
                        AppState.Instance.Settings.Hotkey = value;
                        AppState.Instance.Settings.Save();
                    }

                    OnPropertyChanged("Hotkey");
                    OnPropertyChanged("HotkeyString");
                }
            }
        }

        public string HotkeyString
        {
            get { return (_Hotkey == null ? string.Empty : _Hotkey.ToString()); }
        }

        public bool SaveCardImageData
        {
            get { return AppState.Instance.Settings.SaveCardImageData; }
            set
            {
                if (AppState.Instance.Settings.SaveCardImageData != value) {
                    AppState.Instance.Settings.SaveCardImageData = value;
                    AppState.Instance.Settings.Save();
                    OnPropertyChanged("SaveCardImageData");
                }
            }
        }

        public DisplayViewModel SelectedDisplay
        {
            get { return _SelectedDisplay; }
            set
            {
                if (value != _SelectedDisplay) {
                    _SelectedDisplay = value;
                    AppState.Instance.Settings.DisplayIndex = value.Index;
                    AppState.Instance.Settings.Save();
                    OnPropertyChanged("SelectedDisplay");
                }
            }
        }

        public bool ShowPricingData
        {
            get { return AppState.Instance.Settings.ShowPricingData; }
            set 
            {
                if (AppState.Instance.Settings.ShowPricingData != value) {
                    AppState.Instance.Settings.ShowPricingData = value;
                    AppState.Instance.Settings.Save();
                    OnPropertyChanged("ShowPricingData");
                }
            }
        }

        public bool StartOnSignIn
        {
            get { return AppState.Instance.Settings.StartOnSignIn; }
            set
            {
                if (AppState.Instance.Settings.StartOnSignIn != value) {
                    AppState.Instance.Settings.StartOnSignIn = value;
                    AppState.Instance.Settings.Save();
                    OnPropertyChanged("StartOnSignIn");
                }
            }
        }

        public IEnumerable<TweetViewModel> Tweets
        {
            get { return _Tweets; }
        }

        public string VersionString { get; set; }

        private void GetPackages()
        {
            this.Packages = AppState.Instance.MelekDataStore.GetPackages().OrderBy(p => p.Name).OrderBy(p => p.CardsReleased);
        }

        private void GetTweets()
        {
            BackgroundBuddy.RunAsync(() => {
                TwitterGitter gitter = new TwitterGitter("HgM9fPG8L1ffEtzrVnSgtKLOp", "z5RViBlJahCTaNRAnz8Gy1vrTn420CZ80hReakMXceMJzvSnsz");
                _Tweets = TweetViewModel.FromJson(gitter.GetUserTimeline("jammerware"));
            });
        }

        public void QueryCardCacheSize()
        {
            BackgroundBuddy.RunAsync(() => {
                CardsDirectorySize = AppState.Instance.MelekDataStore.GetCardImageCacheSize();
            });
        }
    }
}