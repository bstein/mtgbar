using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Melek.Domain;
using MtGBar.Infrastructure.Utilities;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardSetImageConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value != null) {
                IPrinting printing = (value as IPrinting);

                try {
                    string localPath = ImageManager.GetSetSymbolPath(printing.Set, printing.Rarity); 
                    Uri uri;

                    if (File.Exists(localPath)) {
                        uri = new Uri(localPath);
                        return new BitmapImage(uri);
                    }
                    else {
                        uri = new Uri(ImageManager.GetSetSymbolUrl(printing.Set, printing.Rarity));
                        // download it for next time
                        ImageManager.DownloadSetSymbol(printing.Set, printing.Rarity);
                    }
                    return new BitmapImage(uri);
                }
                catch (Exception e) {
                    AppState.Instance.LoggingNinja.LogError(e);
                }
            }

            return null;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}