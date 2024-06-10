using Beny.Models;
using System.Globalization;
using System.Windows.Data;

namespace Beny.Converters
{
    internal class ListTagsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<FootballEventTag> tags = (IEnumerable<FootballEventTag>) value;

            if (tags.Any() == false)
            {
                return "Нет";
            }

            return String.Join(", ", tags.Select(x => x.Tag.Name));

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
