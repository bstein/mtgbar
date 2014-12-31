using System;
using System.IO;
using System.Windows.Input;
using MtGBar.Infrastructure.Utilities;
using MtGBar.ViewModels;

namespace MtGBar.Infrastructure.UIHelpers.Commands
{
    public class ClearCardCacheCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            AppState.Instance.MelekDataStore.ClearCardImageCache();

            if (parameter != null && parameter.GetType() == typeof(AboutViewModel)) {
                (parameter as AboutViewModel).QueryCardCacheSize();
            };
        }
    }
}