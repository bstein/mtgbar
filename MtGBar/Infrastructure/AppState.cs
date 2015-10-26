using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Bazam.KeyAdept;
using Bazam.KeyAdept.Infrastructure;
using Hardcodet.Wpf.TaskbarNotification;
using Melek;
using Melek.Client.DataStore;
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
        public MelekClient MelekClient { get; private set; }
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

        private Dispatcher AppDispatcher { get; set; }
        #endregion

        private AppState()
        {
            AppDispatcher = Dispatcher.CurrentDispatcher;
            LoggingNinja = new LoggingNinja(FileSystemManager.LogFileName);
            Settings = new Settings();
            HotkeyRegistrar = new HotkeyRegistrar();
            MelekClient = new MelekClient() {
                StoreCardImagesLocally = true,
                UpdateCheckInterval = TimeSpan.FromMinutes(10)
            };

            // construct the taskbar icon
            this.TaskbarIcon = null;
            ViewCardCommand command = new ViewCardCommand();

            TaskbarIcon icon = new TaskbarIcon();
            icon.ContextMenu = new ContextMenu();
            icon.IconSource = new BitmapImage(new Uri("pack://application:,,,/Assets/taskbar-icon.ico"));
            icon.LeftClickCommand = command;
            icon.ToolTipText = AppConstants.APPNAME + " | loading...";

            this.TaskbarIcon = icon;

            // This is ugly.
            // 
            // Basically, right now the Settings object and the MelekDataStore object have a circular dependency. Settings needs MelekDataStore to make
            // sense of the multiverseIDs it stores as the most recent cards viewed, and MelekDataStore needs to know whether or not it should cache card
            // images locally, a fact that Settings keeps track of. The solution FOR NOW is to expose the method that loads recent cards from the settings
            // file and call it when the data store is ready.
            //
            // But I'm not happy about it, okay?
            // TODO: STOP SUCKING
            MelekClient.DataLoaded += () => {
                Settings.LoadRecentCards();
            };
        }

        public async Task Initialize()
        {
            // load up the client
            await MelekClient.LoadFromDirectory(FileSystemManager.MelekDataDirectory);

            Settings.Updated += (theSettings, omgChanged) => {
                MelekClient.StoreCardImagesLocally = Settings.SaveCardImageData;
                BuildContextMenu(this.TaskbarIcon);
            };
            
            BuildContextMenu(this.TaskbarIcon);
        }
        
        public void RegisterHotkey(HotkeyDescription hkd)
        {
            Hotkey hotkey = hkd.ToHotkey();
            HotkeyRegistrar.RegisterHotkey(hotkey);
            this.Hotkey = hotkey;
        }

        private void BuildContextMenu(TaskbarIcon taskbarItem)
        {
            AppDispatcher.Invoke(new Action(() => {
                ContextMenu menu = taskbarItem.ContextMenu;
                menu.Items.Clear();

                foreach (MenuItem item in GetContextMenuTop()) {
                    if (taskbarItem.ContextMenu != null && taskbarItem.ContextMenu.Items.Contains(item)) taskbarItem.ContextMenu.Items.Remove(item);
                    menu.Items.Add(item);
                }

                if (Settings.RecentCards.Length > 0) {
                    menu.Items.Add(new Separator());

                    menu.Items.Add(new MenuItem() {
                        Header = "Recent cards",
                        IsEnabled = false
                    });

                    foreach (ICard card in Settings.RecentCards) {
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
            }));
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
            AlertView alertView = (App.Current.FindResource("AlertView") as AlertView);
            SearchView searchView = (App.Current.FindResource("SearchView") as SearchView);

            if (searchView.IsActive) {
                searchView.Hide();
            }
            else {
                alertView.Hide();
                searchView.Show();
                searchView.Activate();
            }
        }
    }
}