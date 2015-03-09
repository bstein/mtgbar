using System;
using System.Windows.Input;
using Melek.Models;
using MtGBar.ViewModels;
using MtGBar.Views;

namespace MtGBar.Infrastructure.UIHelpers.Commands
{
    public class ViewCardCommand : ICommand
    {
        private Card Card { get; set; }

        public ViewCardCommand() {}
        public ViewCardCommand(Card card)
        {
            this.Card = card;
        }

        #region ICommand
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        #endregion

        public void Execute(object parameter)
        {
            SearchView view = (App.Current.Resources["SearchView"] as SearchView);
            (view.DataContext as SearchViewModel).SelectedCard = this.Card;
            view.Show();
        }
    }
}