using System;
using System.Diagnostics;
using System.Windows.Input;

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
            if (parameter != null && !string.IsNullOrEmpty(parameter.ToString())) {
                Process proc = new Process();
                ProcessStartInfo info = new ProcessStartInfo(parameter.ToString());
                info.UseShellExecute = true;
                proc.StartInfo = info;
                proc.Start();
            }
        }
    }
}