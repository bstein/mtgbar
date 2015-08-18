using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Melek.Domain;
using MtGBar.Infrastructure.Utilities;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardBackgroundImageConverter : IValueConverter
    {
        private static string RESOLVED_BACKGROUND = null;

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Uri uri = null;
            IPrinting typedValue = value as IPrinting;

            if (value != null && !string.IsNullOrEmpty((value as IPrinting).Watermark)) {
                uri = new Uri("pack://application:,,,/Assets/backgrounds/" + typedValue.Watermark.ToLower() + ".jpg", UriKind.Absolute);
            }
            else {
                if (string.IsNullOrEmpty(RESOLVED_BACKGROUND)) {
                    IList<Set> sets = AppState.Instance.MelekClient.GetSets().OrderByDescending(s => s.Date).ToList();
                    string localPath = string.Empty;

                    foreach (Set set in sets) {
                        string setArtPath = Path.Combine(FileSystemManager.SetArtDirectory, set.Code + ".jpg");
                        if (File.Exists(setArtPath)) {
                            RESOLVED_BACKGROUND = setArtPath;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(RESOLVED_BACKGROUND)) {
                    uri = new Uri(RESOLVED_BACKGROUND);
                }
            }

            if (uri == null) {
                uri = new Uri("pack://application:,,,/Assets/backgrounds/default.jpg");
            }

            BitmapImage bmp = new BitmapImage(uri);
            return new BitmapImage(uri);
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}