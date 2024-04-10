using Beny.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Beny.Converters
{
    internal class ListForecastsToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<FootballEvent> events = (IEnumerable<FootballEvent>)value;

            int maxCount = int.Parse(parameter.ToString());

            if (events.Count() > 1)
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (FootballEvent footEvent in events.Take(maxCount))
                {
                    stringBuilder.Append(footEvent.Forecast.Name + ", ");
                }

                if (events.Count() > maxCount)
                {
                    stringBuilder.Append("...");
                }
                else
                {
                    stringBuilder.Remove(stringBuilder.Length - 2, 2);
                }

                return stringBuilder.ToString();
            }

            var footballEvent = events.FirstOrDefault();

            return $"{footballEvent?.Forecast.Name}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
