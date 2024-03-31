using Beny.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beny.Models
{
    public class Bet : BindableBase
    { 
        public int Id { get; set; }

        public ObservableCollection<FootballEvent> FootballEvents { get; set; } = new ObservableCollection<FootballEvent>();

        public DateTime CreatedAt { get; set; }

        public float Coefficient
        {
            get
            {
                float result = 1.0f;

                foreach (FootballEvent footballEvent in FootballEvents)
                {
                    result *= footballEvent.Coefficient;
                }

                return result;
            }
        }

        public Bet()
        {
            FootballEvents.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Coefficient));
            };
        }
    }
}
