using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Bazam.KeyAdept;
using Bazam.KeyAdept.Infrastructure;
using Hardcodet.Wpf.TaskbarNotification;
using Melek.DataStore;
using Melek.Utilities;
using MtGBar.Infrastructure.DataNinjitsu.Models;
using MtGBar.Infrastructure.UIHelpers.Commands;
using MtGBar.Infrastructure.Utilities;
using MtGBar.Views;

namespace MtGBar.Infrastructure
{
    public sealed class AppState
    {
        #region Singleton
        private static readonly AppState _Instance = new AppState();
        public static AppState Instance
        {
            get { return _Instance; }
        }
        #endregion

        #region Properties
        public HotkeyRegistrar HotkeyRegistrar { get; private set; }
        public LoggingNinja LoggingNinja { get; private set; }
        public MelekDataStore MelekDataStore { get; private set; }
        public Settings Settings { get; private set; }
        public TaskbarIcon TaskbarIcon { get; private set; }

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

        private AppState()
        {
            LoggingNinja = new LoggingNinja(FileSystemManager.LogFileName);
            Settings = new Settings();
            HotkeyRegistrar = new HotkeyRegistrar();

            if (string.IsNullOrEmpty(Settings.MelekDevAuthkey)) {
                MelekDataStore = new MelekDataStore(FileSystemManager.MelekDataDirectory, Settings.SaveCardImageData, LoggingNinja);
            }
            else {
                MelekDataStore = new MelekDataStore(FileSystemManager.MelekDataDirectory, Settings.SaveCardImageData, LoggingNinja, false, Settings.MelekDevAuthkey);
            }

            Settings.Updated += (theSettings, omgChanged) => {
                MelekDataStore.StoreCardImagesLocally = Settings.SaveCardImageData;
            };

            // construct the taskbar icon
            LaunchCommand command = new LaunchCommand();

            TaskbarIcon icon = new TaskbarIcon();
            icon.IconSource = new BitmapImage(new Uri("pack://application:,,,/Assets/taskbar-icon.ico"));
            icon.LeftClickCommand = command;
            icon.ToolTipText = AppConstants.APPNAME + " | loading...";

            ContextMenu menu = new ContextMenu();
            menu.Items.Add(new MenuItem() {
                Command = command,
                Header = "Browse"
            });
            menu.Items.Add(new MenuItem() {
                Command = new AboutCommand(),
                Header = "Settings"
            });
            menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem() {
                Command = new CloseCommand(),
                Header = "Exit"
            });

            icon.ContextMenu = menu;
            this.TaskbarIcon = icon;
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