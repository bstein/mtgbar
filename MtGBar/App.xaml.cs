using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Bazam.KeyAdept.Infrastructure;
using FirstFloor.ModernUI.Presentation;
using MtGBar.Infrastructure;
using MtGBar.Infrastructure.Utilities;
using MtGBar.Infrastructure.Utilities.Updates;
using MtGBar.ViewModels;
using MtGBar.Views;

namespace MtGBar
{
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            AppState.Instance.LoggingNinja.LogMessage("UNCAUGHT: " + e.Exception.Message + " STACK TRACE: " + e.Exception.StackTrace);
            e.Handled = true;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AppState.Instance.HotkeyRegistrar.UnregisterAllHotkeys();
        }

        private void MelekClient_Loaded()
        {
            Dispatcher.Invoke(() => {
                AppState.Instance.TaskbarIcon.ToolTipText = AppConstants.APPNAME;
            });

            // start the updater clock so it polls for updates
            Updater.Instance.UpdateFound += () => {
                TalkAtcha.TalkAtEm("Update!", "There's an update available for " + AppConstants.APPNAME + "! You can download it as quick as instant-speed removal by closing and reopening the app.");
            };
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // first of all, omg, make sure there's only one instance running. it gets weird fast otherwise.
            Process thisProc = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1) {
                MessageBox.Show("Oops. " + AppConstants.APPNAME + " is already running. Check your System Tray and you should should see it there. If you're having trouble, close all instances of MtGBar, start it up again, and use the \"Settings\" menu item to contact the developer. Sorry!", AppConstants.APPNAME);
                Application.Current.Shutdown();
                return;
            }

            // make sure our user app data stuff is there
            FileSystemManager.Init();

            // as soon as a melekdatastore instance is referenced, it'll start loading, so know that shit and register a listener that will do stuff when it's done
            AppState.Instance.MelekClient.DataLoaded += MelekClient_Loaded;

            // TODO: need MelekClient to raise an event when new data is downloaded?
            // also say my name say my name when the thing has new packages
            //AppState.Instance.MelekDataStore.PackagesUpdated += (newPackages) => {
            //    // set the lastimagecheck to a low value so that next time the clock comes around, new image data is downloaded (for the new packages)
            //    AppState.Instance.Settings.LastImageCheck = DateTime.MinValue;
            //    AppState.Instance.Settings.Save();

            //    if (newPackages.Length == 1) {
            //        TalkAtcha.TalkAtEm("New data: " + newPackages[0].Name, AppConstants.APPNAME + " has downloaded new data about " + newPackages[0].Name + ". Search for your favorite spoiler now!");
            //    }
            //    else if (newPackages.Length > 1) {
            //        TalkAtcha.TalkAtEm("New data!", AppConstants.APPNAME + " just downloaded a bunch of new data. Search for your favorite new card now!");
            //    }
            //};

            // HEY! LISTEN!
            // ... to the hotkey registrar so we can tell the user if they try to do something that will screw stuff up
            AppState.Instance.HotkeyRegistrar.UnavailableHotkeyRegistered += (HotkeyEventArgs args) => {
                TalkAtcha.TalkAtEm("Oops.", AppConstants.APPNAME + " tried to register its hotkey but couldn't. This is usually because it's trying to use a hotkey that some other software is using. Try visiting settings and changing the hotkey. Sorry :(");
            };

            // pretty sure here's where we should hook up the hotkey
            if (AppState.Instance.Settings.Hotkey != null) {
                AppState.Instance.RegisterHotkey(AppState.Instance.Settings.Hotkey);
            }

            // set up MUI theme
            Color myAccentColor = (Color)App.Current.FindResource("MyAccentColor");
            AppearanceManager.Current.AccentColor = myAccentColor;

            // we're ready - tell them what's going on
            if (AppState.Instance.Settings.ShowWelcomeScreen) {
                Dispatcher.Invoke(() => {
                    AlertView welcomeView = FindResource("AlertView") as AlertView;
                    welcomeView.DataContext = new WelcomeViewModel();
                    welcomeView.Show();
                });
            }
        }
    }
}