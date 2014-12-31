using System;
using System.Diagnostics;
using System.Windows.Input;
using MtGBar.Infrastructure.Utilities;
using Melek;

namespace MtGBar.Infrastructure.UIHelpers.Commands
{
    public class GathererViewCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Process.Start(VendorRelations.GetGathererLink(parameter.ToString()));
        }
    }
}