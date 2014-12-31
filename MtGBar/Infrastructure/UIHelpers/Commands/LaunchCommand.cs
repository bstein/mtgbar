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
            // for some reason App.xaml binds the CanExecute property of this command before it creates the SearchView object, even 
            // though the SearchView is defined first. Dumb.
            // return parameter != null && parameter.GetType() == typeof(SearchView);
            return true;
        }

        public void Execute(object parameter)
        {
            (parameter as SearchView).Show();
        }
    }
}