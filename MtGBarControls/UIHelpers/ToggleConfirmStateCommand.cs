using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using System.Windows;

namespace MtGBarControls.UIHelpers
{
    public class ToggleConfirmStateCommand : ICommand
    {
        private enum ConfirmState
        {
            Confirming,
            NotConfirming
        }

        private ConfirmState _CurrentState = ConfirmState.NotConfirming;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            FrameworkElement typedParam = (parameter as FrameworkElement);

            if (_CurrentState == ConfirmState.NotConfirming) {
                VisualStateManager.GoToState(parameter as FrameworkElement, ConfirmState.Confirming.ToString(), true);
                _CurrentState = ConfirmState.Confirming;
            }
            else {
                VisualStateManager.GoToState(parameter as FrameworkElement, ConfirmState.NotConfirming.ToString(), true);
                _CurrentState = ConfirmState.NotConfirming;
            }
        }
    }
}
