using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Bazam.KeyAdept;
using Bazam.KeyAdept.Infrastructure;
using Hardcodet.Wpf.TaskbarNotification;
using Melek.DataStore;
using Melek.Models;
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
                MelekDataStore = new MelekDataStore(FileSystemManager.MelekDataDirectory, true, LoggingNinja);
            }
            else {
                MelekDataStore = new MelekDataStore(FileSystemManager.MelekDataDirectory, true, LoggingNinja, false, Settings.MelekDevAuthkey);
            }

            // This is ugly.
            // 
            // Basically, right now the Settings object and the MelekDataStore object have a circular dependency. Settings needs MelekDataStore to make
            // sense of the multiverseIDs it stores as the most recent cards viewed, and MelekDataStore needs to know whether or not it should cache card
            // images locally, a fact that Settings keeps track of. The solution for now is to expose the method that loads recent cards from the settings
            // file and call it when the data store is ready.
            //
            // But I'm not happy about it, okay?
            MelekDataStore.DataLoaded += (omg, theDataIsReadyGurl) => { Settings.LoadRecentCards(); };
            
            Settings.Updated += (theSettings, omgChanged) => {
                MelekDataStore.StoreCardImagesLocally = Settings.SaveCardImageData;
                BuildContextMenu(this.TaskbarIcon);
            };

            // construct the taskbar icon
            this.TaskbarIcon = null;
            ViewCardCommand command = new ViewCardCommand();

            TaskbarIcon icon = new TaskbarIcon();
            icon.IconSource = new BitmapImage(new Uri("pack://application:,,,/Assets/taskbar-icon.ico"));
            icon.LeftClickCommand = command;
            icon.ToolTipText = AppConstants.APPNAME + " | loading...";

            BuildContextMenu(icon);
            this.TaskbarIcon = icon;
        }

        public void RegisterHotkey(HotkeyDescription hkd)
        {
            Hotkey hotkey = hkd.ToHotkey();
            HotkeyRegistrar.RegisterHotkey(hotkey);
            this.Hotkey = hotkey;
        }

        private void BuildContextMenu(TaskbarIcon taskbarItem)
        {
            ContextMenu menu = new ContextMenu();

            foreach (MenuItem item in GetContextMenuTop()) {
                if (taskbarItem.ContextMenu != null && taskbarItem.ContextMenu.Items.Contains(item)) taskbarItem.ContextMenu.Items.Remove(item);
                menu.Items.Add(item);
            }

            if (Settings.RecentCards.Length > 0) {
                menu.Items.Add(new Separator());

                foreach (Card card in Settings.RecentCards) {
                    menu.Items.Add(new MenuItem() {
                        Command = new ViewCardCommand(card),
                        Header = card.Name
                    });
                }
            }

            menu.Items.Add(new Separator());
            foreach (MenuItem item in GetContextMenuBottom()) {
                menu.Items.Add(item);
            }

            taskbarItem.ContextMenu = menu;
        }

        private MenuItem[] GetContextMenuTop()
        {
            return new MenuItem[] {
                new MenuItem() {
                    Command = new ViewCardCommand(),
                    Header = "Browse"
                },
                new MenuItem() {
                    Command = new AboutCommand(),
                    Header = "Settings"
                }
            };
        }

        private MenuItem[] GetContextMenuBottom()
        {
            return new MenuItem[] {
                new MenuItem() {
                    Command = new CloseCommand(),
                    Header = "Exit"
                }
            };
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