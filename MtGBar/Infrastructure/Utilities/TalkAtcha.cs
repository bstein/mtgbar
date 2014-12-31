using System;
using System.Drawing;
using System.IO;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace MtGBar.Infrastructure.Utilities
{
    public static class TalkAtcha
    {
        private static TaskbarIcon _TheTaskBarIcon = null;

        public static void TalkAtEm(string title, string message)
        {
            if (_TheTaskBarIcon == null) {
                _TheTaskBarIcon = (TaskbarIcon)App.Current.FindResource("TheTaskBarIcon");
            }

            using(Stream iconStream = Application.GetResourceStream(new Uri("/Assets/taskbar-icon.ico", UriKind.Relative)).Stream) {
                _TheTaskBarIcon.ShowBalloonTip(title, message, new Icon(iconStream));
            }
        }
    }
}