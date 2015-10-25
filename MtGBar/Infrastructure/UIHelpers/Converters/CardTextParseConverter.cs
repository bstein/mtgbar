using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using Melek.Domain;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class CardTextParseConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            List<object> pieces = new List<object>();
            string sValue = value.ToString();
            if (value != null) {
                foreach (string piece in Regex.Split(sValue, @"(\{\S+?\})|(\s)")) {
                    Match match = Regex.Match(piece, @"\{(\S+?)\}");
                    if (match != null && match.Groups[1].Value != string.Empty) {
                        pieces.Add(new CardCost(match.Groups[1].Value));
                    }
                    else if (!string.IsNullOrEmpty(piece)) {
                        pieces.Add(piece);
                    }
                }
                return pieces;
            }
            return new string[] { };
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
