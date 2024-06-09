using Beny.Base;
using Beny.Collections;
using Beny.Enums;
using System.Collections.ObjectModel;

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

        public Bet()
        {
            FootballEvents.CollectionChanged += (_, _) => Update();
            FootballEvents.ItemPropertyChanged += (_, _) => Update();
        }

        private void Update()
        {
            OnPropertyChanged(nameof(FootballEvents));
            OnPropertyChanged(nameof(Coefficient));
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(IsExpress));
        }
    }
}
