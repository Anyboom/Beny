using System.Globalization;
using System.Windows.Data;

namespace Beny.Helpers.Converters
{
    class BooleanTypeFootballEventToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool IsExpress = (bool)value;

            if (IsExpress)
            {
                return "Экспресс";
            }
            else
            {
                return "Ординар";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
