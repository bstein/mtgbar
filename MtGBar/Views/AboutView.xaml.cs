using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using MtGBar.ViewModels;

namespace MtGBar.Views
{
    public partial class AboutView : ModernWindow
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private async void this_Loaded(object sender, RoutedEventArgs e)
        {
            await (DataContext as AboutViewModel).Load();
        }
    }
}