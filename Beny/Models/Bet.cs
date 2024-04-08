using Beny.Base;
using Beny.Collections;
using Beny.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Beny.Models
{
    public class Bet : BindableBase
    {
        private int _Id;
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public ItemObservableCollection<FootballEvent> FootballEvents { get; set; } = new ItemObservableCollection<FootballEvent>();

        public DateTime CreatedAt { get; set; }

        public float? Coefficient
        {
            get
            {
                float? result = 1.0f;

                foreach (FootballEvent footballEvent in FootballEvents)
                {
                    result *= footballEvent.Coefficient;
                }

                return result;
            }
        }

        public FootballEventStatus Status
        {
            get
            {
                if (FootballEvents.Any(x => x.FootballEventStatus == FootballEventStatus.Lose))
                {
                    return FootballEventStatus.Lose;
                }

                if (FootballEvents.All(x => x.FootballEventStatus != FootballEventStatus.Lose && x.FootballEventStatus != FootballEventStatus.NotCalculated))
                {
                    return FootballEventStatus.Win;
                }

                if (FootballEvents.All(x => x.FootballEventStatus == FootballEventStatus.Return))
                {
                    return FootballEventStatus.Return;
                }

                return FootballEventStatus.NotCalculated;
            }
        }

        public bool IsExpress
        {
            get
            {
                return FootballEvents.Count > 1;
            }
        }

        public string NameType
        {
            get
            {
                if (IsExpress)
                {
                    return "Экспресс";
                }
                return "Ординар";
            }
        }

        public string NameFootballEvent
        {
            get
            {
                if (IsExpress)
                {
                    return $"Экспресс из {FootballEvents.Count} событий";
                }

                var footballEvent = FootballEvents.FirstOrDefault();

                return $"{footballEvent?.HomeTeam} - {footballEvent?.GuestTeam}";
            }
        }

        public string NameForecasts
        {
            get
            {
                if (IsExpress)
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    foreach (FootballEvent footEvent in FootballEvents.Take(5))
                    {
                        stringBuilder.Append(footEvent.Forecast.Name + ", ");
                    }

                    if (FootballEvents.Count > 5)
                    {
                        stringBuilder.Append("...");
                    }
                    else
                    {
                        stringBuilder.Remove(stringBuilder.Length - 2, 2);
                    }

                    return stringBuilder.ToString();
                }

                var footballEvent = FootballEvents.FirstOrDefault();

                return $"{footballEvent?.Forecast.Name}";
            }
        }

        public Bet()
        {
            FootballEvents.CollectionChanged += (s, e) => Update();
            FootballEvents.ItemPropertyChanged += (s, e) => Update();
        }

        private void Update()
        {
            OnPropertyChanged(nameof(Coefficient));
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(FootballEvents));
            OnPropertyChanged(nameof(IsExpress));
            OnPropertyChanged(nameof(NameForecasts));
            OnPropertyChanged(nameof(NameFootballEvent));
            OnPropertyChanged(nameof(NameType));
        }
    }
}
