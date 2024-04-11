using System;
using System.Globalization;
using System.Windows.Data;

namespace prbd_2324_g01.ViewModel {
    public class SignConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null || !(value is double))
                return 0;

            double number = (double)value;
            return Math.Sign(number);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}