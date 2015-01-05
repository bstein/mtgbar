using System;
using System.Windows.Input;
using MtGBar.Views;

namespace MtGBar.Infrastructure.UIHelpers.Commands
{
    public class LaunchCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            (App.Current.Resources["SearchView"] as SearchView).Show();
        }
    }
}