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
        private static string RESOLVED_BACKGROUND = string.Empty;

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Uri uri = null;

            if (value != null && !string.IsNullOrEmpty((value as ICard).Watermark)) {
                uri = new Uri("pack://application:,,,/Assets/backgrounds/" + (value as ICard).Watermark.ToLower() + ".jpg", UriKind.Absolute);
            }
            else {
                if (string.IsNullOrEmpty(RESOLVED_BACKGROUND)) {
                    List<Package> packages = new List<Package>(AppState.Instance.MelekClient.GetPackages().OrderByDescending(p => p.DataUpdated));
                    string localPath = string.Empty;

                    foreach (Package package in packages) {
                        string packageArtPath = Path.Combine(FileSystemManager.PackageArtDirectory, package.ID + ".jpg");
                        if (File.Exists(packageArtPath)) {
                            RESOLVED_BACKGROUND = packageArtPath;
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