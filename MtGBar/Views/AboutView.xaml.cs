using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Bazam.KeyAdept.Infrastructure;
using MtGBar.Infrastructure.DataNinjitsu.Models;
using MtGBar.ViewModels;

namespace MtGBar.Views
{
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void Window_Activated(object sender, RoutedEventArgs e)
        {
            (DataContext as AboutViewModel).QueryCardCacheSize();
        }

        private void HotBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            HotkeyDescription hkd = null;
            if (e.Key != System.Windows.Input.Key.Escape && e.Key != System.Windows.Input.Key.Back) {
                List<Modifier> modifiers = new List<Modifier>();
                if (e.Key == System.Windows.Input.Key.System) {
                    return;
                }

                // Fetch the actual shortcut key.
                System.Windows.Input.Key key = (e.Key == System.Windows.Input.Key.System ? e.SystemKey : e.Key);

                if (key == System.Windows.Input.Key.LeftShift || key == System.Windows.Input.Key.RightShift
                    || key == System.Windows.Input.Key.LeftCtrl || key == System.Windows.Input.Key.RightCtrl
                    || key == System.Windows.Input.Key.LeftAlt || key == System.Windows.Input.Key.RightAlt
                    || key == System.Windows.Input.Key.LWin || key == System.Windows.Input.Key.RWin) {
                    return;
                }

                StringBuilder shortcutText = new StringBuilder();
                if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0) {
                    modifiers.Add(Modifier.Alt);
                }
                if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) {
                    modifiers.Add(Modifier.Ctrl);
                }
                if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) {
                    modifiers.Add(Modifier.Shift);
                }
                if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0) {
                    modifiers.Add(Modifier.Win);
                }

                hkd = new HotkeyDescription(modifiers, (Bazam.KeyAdept.Infrastructure.Key)KeyInterop.VirtualKeyFromKey(e.Key));
            }

             if (hkd == null || hkd.Modifiers.Count > 0) {
                 (DataContext as AboutViewModel).Hotkey = hkd;
             }
        }
    }
}