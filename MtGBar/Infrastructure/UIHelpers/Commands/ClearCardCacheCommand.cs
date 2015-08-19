using System;
using System.Windows.Input;
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

        public async void Execute(object parameter)
        {
            await AppState.Instance.MelekClient.ClearCardImageCache();

            if (parameter != null && parameter.GetType() == typeof(AboutViewModel)) {
                await (parameter as AboutViewModel).LoadCardCacheSize();
            };
        }
    }
}