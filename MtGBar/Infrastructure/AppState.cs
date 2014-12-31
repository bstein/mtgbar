using Bazam.KeyAdept;
using Bazam.KeyAdept.Infrastructure;
using Melek.Utilities;
using MtGBar.Infrastructure.DataNinjitsu.Models;
using MtGBar.Infrastructure.Utilities;
using MtGBar.Views;

namespace MtGBar.Infrastructure
{
    public class AppState
    {
        #region Singleton
        private static AppState _Instance;
        public static AppState Instance
        {
            get
            {
                if (_Instance == null) {
                    _Instance = new AppState();
                }
                return _Instance;
            }
        }
        #endregion

        #region Properties
        public HotkeyRegistrar HotkeyRegistrar { get; private set; }
        public LoggingNinja LoggingNinja { get; private set; }
        public MelekDataStore MelekDataStore { get; private set; }
        public Settings Settings { get; private set; }

        private Hotkey _Hotkey;
        public Hotkey Hotkey 
        {
            get { return _Hotkey; }
            private set
            {
                if (_Hotkey != null) {
                    _Hotkey.Pressed -= thisHotkey_Pressed;
                }

                _Hotkey = value;
                _Hotkey.Pressed += thisHotkey_Pressed;
            }
        }
        #endregion

        public AppState()
        {
            Settings = new Settings();
            HotkeyRegistrar = new HotkeyRegistrar();
            LoggingNinja = new LoggingNinja(FileSystemManager.LogFileName);
            MelekDataStore = new MelekDataStore(FileSystemManager.MelekDataDirectory, Settings.SaveCardImageData, LoggingNinja);

            Settings.Updated += (theSettings, omgChanged) => {
                MelekDataStore.StoreCardImagesLocally = Settings.SaveCardImageData;
            };
        }

        public void RegisterHotkey(HotkeyDescription hkd)
        {
            Hotkey hotkey = hkd.ToHotkey();
            HotkeyRegistrar.RegisterHotkey(hotkey);
            this.Hotkey = hotkey;
        }

        private void thisHotkey_Pressed(HotkeyPressedEventArgs args)
        {
            SearchView searchView = (App.Current.FindResource("SearchView") as SearchView);
            if (searchView.IsActive) {
                searchView.Hide();
            }
            else {
                searchView.Show();
                searchView.Activate();
            }
        }
    }
}