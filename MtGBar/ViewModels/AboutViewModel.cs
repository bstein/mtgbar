using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using Bazam.Wpf.ViewModels;
using Bazam.KeyAdept;
using Bazam.Modules;
using Melek.Domain;
using MtGBar.Infrastructure;
using MtGBar.Infrastructure.DataNinjitsu.Models;
using MtGBar.Infrastructure.UIHelpers.Commands;
using MtGBar.Infrastructure.Utilities;
using System.Threading.Tasks;
using Bazam.Twitter;

namespace MtGBar.ViewModels
{
    public class AboutViewModel : ViewModelBase<AboutViewModel>
    {
        private string _CardsDirectorySize = string.Empty;
        private ICommand _ClearAppDataCacheCommand = new ClearAppDataCacheCommand();
        private ICommand _ClearCardCacheCommand = new ClearCardCacheCommand();
        private DisplayViewModel[] _DisplayViewModels;
        private HotkeyDescription _Hotkey { get; set; }
        private string _HotkeyString { get; set; }
        private DisplayViewModel _SelectedDisplay = null;
        public IEnumerable<TweetViewModel> _Tweets = new List<TweetViewModel>();

        public AboutViewModel()
        {
            // set hotkey
            _Hotkey = AppState.Instance.Settings.Hotkey;
            
            // find out about the card cache size
            QueryCardCacheSize().GetAwaiter();

            // set version
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            try {
                version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception) { }
            VersionString = "v " + version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Revision.ToString();
            
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
            set { ChangeProperty(vm => vm.CardsDirectorySize, value); }
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
                    RaisePropertyChanged("DismissOnFocusLoss");
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

                    RaisePropertyChanged("Hotkey");
                    RaisePropertyChanged("HotkeyString");
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
                    RaisePropertyChanged("SaveCardImageData");
                }
            }
        }

        public bool ShowWelcomeScreen
        {
            get { return AppState.Instance.Settings.ShowWelcomeScreen; }
            set
            {
                if (AppState.Instance.Settings.ShowWelcomeScreen != value) {
                    AppState.Instance.Settings.ShowWelcomeScreen = value;
                    AppState.Instance.Settings.Save();
                    RaisePropertyChanged("ShowWelcomeScreen");
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
                    RaisePropertyChanged("SelectedDisplay");
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
                    RaisePropertyChanged("ShowPricingData");
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
                    RaisePropertyChanged("StartOnSignIn");
                }
            }
        }

        public int TweetCount
        {
            get { return _Tweets.Count(); }
        }

        public IEnumerable<TweetViewModel> Tweets
        {
            get { return _Tweets; }
            private set
            {
                _Tweets = value;
                RaisePropertyChanged("Tweets");
                RaisePropertyChanged("TweetCount");
            }
        }

        public string VersionString { get; set; }
        
        private async Task GetTweets()
        {
            TwitterGitter gitter = new TwitterGitter("HgM9fPG8L1ffEtzrVnSgtKLOp", "z5RViBlJahCTaNRAnz8Gy1vrTn420CZ80hReakMXceMJzvSnsz");
            Dictionary<long, TweetViewModel> tweets = new Dictionary<long, TweetViewModel>();

            foreach (TweetViewModel tweet in TweetViewModel.FromTimelineJson(await gitter.GetUserTimeline("jammerware"))) {
                tweets.Add(tweet.TweetID, tweet);
            }

            foreach (TweetViewModel tweet in TweetViewModel.FromJson(await gitter.Search("@jammerware"))) {
                if (!tweets.Keys.Contains(tweet.TweetID)) {
                    tweets.Add(tweet.TweetID, tweet);
                }
            }

            Tweets = tweets.Values.OrderByDescending(t => t.Date);
        }

        public async Task QueryCardCacheSize()
        {
            CardsDirectorySize = await AppState.Instance.MelekClient.GetCardImageCacheSize(true);
            CardsDirectorySize = await AppState.Instance.MelekClient.GetCardImageCacheSize();
        }
    }
}