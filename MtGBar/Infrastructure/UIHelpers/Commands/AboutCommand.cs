using System;
using System.Windows.Input;
using MtGBar.Views;

namespace MtGBar.Infrastructure.UIHelpers.Commands
{
    public class AboutCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            AboutView view = new AboutView();
            view.Show();
        }
    }
}