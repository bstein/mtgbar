using System;
using System.Diagnostics;
using System.Windows.Input;
using MtGBar.Infrastructure.Utilities;

namespace MtGBar.Infrastructure.UIHelpers.Commands
{
    public class ClearAppDataCacheCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try {
                AppState.Instance.Settings.LastImageCheck = DateTime.MinValue;
                AppState.Instance.Settings.Save();

                Process p = new Process();
                string args = "\"" + AppState.Instance.MelekDataStore.PackagesDirectory + "\" \"" + FileSystemManager.PackageArtDirectory +"\" \"" + FileSystemManager.SetSymbolsDirectory + "\"";
                Debug.WriteLine(args);
                ProcessStartInfo info =new ProcessStartInfo("MtGBarRepairbot.exe", args);
                info.UseShellExecute = false;
                p.StartInfo = info;
                p.Start();
                App.Current.Shutdown();
            }
            catch (Exception e) {
                AppState.Instance.LoggingNinja.LogError(e);
            }
        }
    }
}