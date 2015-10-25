using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MtGBar.Infrastructure.UIHelpers.Converters
{
    public class BooleanValuesConverter : DependencyObject, IMultiValueConverter
    {   
        /// <summary>
        /// Expects three values - in order, a boolean, and two objects. If the boolean, return the first of the objects. If not, the second. Optionally, set the TransformMethod
        /// property of this class to run each object through a transformation before returning.
        /// </summary>
        /// <param name="values">Values to convert.</param>
        /// <param name="targetType">The type of the expected output.</param>
        /// <param name="parameter">An additional parameter that I'm ignoring and then LOLing about.</param>
        /// <param name="culture">See "parameter".</param>
        /// <returns>One of the second two members of the "Values" collection.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool pivot = (bool)values[0];
            Func<object, object> transformMethod = (object arg) => { return arg; };

            if(values.Length >= 4) {
                transformMethod = (Func<object, object>)values[3];
            }

            return transformMethod((pivot ? values[1] : values[2]));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}