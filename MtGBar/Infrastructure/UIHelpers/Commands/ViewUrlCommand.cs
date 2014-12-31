using System;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Win32;

namespace MtGBar.Infrastructure.UIHelpers.Commands
{
    public class ViewUrlCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter != null && parameter.ToString() != string.Empty) {
                Process proc = new Process();
                ProcessStartInfo info = new ProcessStartInfo(parameter.ToString());
                info.UseShellExecute = true;
                proc.StartInfo = info;
                proc.Start();
            }
        }
    }
}