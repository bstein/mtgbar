using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bazam.Wpf.Modules;
using Jammerware.Utilities;
using MtGBar.Infrastructure;
using MtGBar.ViewModels;

namespace MtGBar.Views
{
    public partial class SearchView : Window
    {
        public SearchView()
        {
            InitializeComponent();

            AppState.Instance.Settings.Updated += (omg, newSettings) => {
                SetTaskbarVisibility();
            };

            this.Deactivated += (lol, ohno) => {
                if (AppState.Instance.Settings.DismissOnFocusLoss) {
                    if (DataContext != null) {
                        (DataContext as SearchViewModel).SearchTerm = string.Empty;
                        HideThis();
                    }
                }
            };

            SetTaskbarVisibility();
        }

        private SearchViewModel ViewModel
        {
            get
            {
                if (DataContext != null) {
                    return DataContext as SearchViewModel;
                }
                return null;
            }
        }

        private void CardPicked(object sender, EventArgs e)
        {
            Kontroller.Blur(TheTextBox);
        }

        private void HandleKeyEvents(Key e)
        {
            switch (e) {
                case Key.Escape:
                    HideThis();
                    break;
                case Key.Down:
                    NextListItem(lstResults, true);
                    break;
                case Key.Up:
                    NextListItem(lstResults, false);
                    break;
                case Key.Right:
                    NextListItem(lstPrintings, true);
                    break;
                case Key.Left:
                    NextListItem(lstPrintings, false);
                    break;
                case Key.Return:
                    ViewModel.SelectedCard = (lstResults.SelectedItem as CardViewModel).Card;
                    break;
            }
        }

        private void HideThis()
        {
            AnimationBuddy.Animate(this, "Opacity", 0, 150, (fk, myLife) => {
                TheGrid.Opacity = 0;
                this.Hide();
            });
        }

        private void NextListItem(ListBox box, bool forward)
        {
            Kontroller.Blur(TheTextBox);
            if (forward) {
                if (box.SelectedIndex < box.Items.Count - 1) {
                    box.SelectedIndex++;
                }
                else {
                    box.SelectedIndex = 0;
                }
            }
            else {
                if (box.SelectedIndex == -1 || box.SelectedIndex == 0) {
                    box.SelectedIndex = box.Items.Count - 1;
                }
                else {
                    box.SelectedIndex--;
                }
            }
        }

        private void SetTaskbarVisibility()
        {
            ShowInTaskbar = !AppState.Instance.Settings.DismissOnFocusLoss;
        }

        private void this_Activated(object sender, EventArgs e)
        {
            // pretty fades
            AnimationBuddy.Animate(this, "Opacity", 1, 150, (lol, wut) => {
                AnimationBuddy.Animate(TheGrid, "Opacity", 1, 150);
            });

            // make sure we're focused right
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            TheTextBox.Focus();

            // handle the vm's CardSelectedEvent
            ViewModel.CardSelected -= CardPicked;
            ViewModel.CardSelected += CardPicked;
        }

        private void this_KeyUp(object sender, KeyEventArgs e)
        {
            HandleKeyEvents(e.Key);
        }

        private void this_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // this allows me to use the left and right arrow keys to page through the card's printings. 
            // if they're hitting one of those keys, let the event go through. Otherwise, refocus the textbox
            if (e.Key != Key.Left && e.Key != Key.Right) {
                TheTextBox.Focus();
            }
        }

        private void lstResults_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HandleKeyEvents(Key.Enter);
            TheTextBox.Focus();
        }
    }
}