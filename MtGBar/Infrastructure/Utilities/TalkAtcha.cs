using System;
using System.Drawing;
using System.IO;
using System.Windows;

namespace MtGBar.Infrastructure.Utilities
{
    public static class TalkAtcha
    {
        public static void TalkAtEm(string title, string message)
        {
            using(Stream iconStream = Application.GetResourceStream(new Uri("/Assets/taskbar-icon.ico", UriKind.Relative)).Stream) {
                AppState.Instance.TaskbarIcon.ShowBalloonTip(title, message, new Icon(iconStream));
            }
        }
    }
}