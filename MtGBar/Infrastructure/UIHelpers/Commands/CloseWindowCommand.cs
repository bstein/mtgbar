using System;
using System.Windows;
using System.Windows.Input;

namespace MtGBar.Infrastructure.UIHelpers.Commands
{
    public class CloseWindowCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            (parameter as Window).Hide();
        }
    }
}