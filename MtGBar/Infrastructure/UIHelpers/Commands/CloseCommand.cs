using System;
using System.Windows.Input;
using MtGBar;

namespace MtGBar.Infrastructure.UIHelpers.Commands
{
    public class CloseCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            App.Current.Shutdown();
        }
    }
}
