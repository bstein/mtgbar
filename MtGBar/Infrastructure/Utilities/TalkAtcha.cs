using MtGBar.ViewModels;
using MtGBar.Views;

namespace MtGBar.Infrastructure.Utilities
{
    public static class TalkAtcha
    {
        public static void TalkAtEm(string title, string message)
        {
            AlertView view = (App.Current.FindResource("AlertView") as AlertView);
            view.DataContext = new AlertViewModel() {
                ContentSource = "/Views/AlertViews/GenericAlertView.xaml",
                Message = message,
                WindowTitle = "mtgbar says...",
                WindowSubTitle = title
            };
            view.Show();
        }
    }
}