using System;
using System.Globalization;
using System.Windows.Data;
using Melek.Domain;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class PrintingImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null && (value as IPrinting) != null) {
                return AppState.Instance.MelekClient.GetCardImageUri(value as IPrinting);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
