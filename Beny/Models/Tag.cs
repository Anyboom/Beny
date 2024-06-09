using Beny.Base;
using Beny.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beny.Models
{
    public class Tag : BindableBase, IDictionaryModel
    {
        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;

                OnPropertyChanged(nameof(Id));
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

                OnPropertyChanged(nameof(Name));
            }
        }

        private DateTime? _deletedAt;
        public DateTime? DeletedAt
        {
            get
            {
                return _deletedAt;
            }
            set
            {
                _deletedAt = value;

                OnPropertyChanged(nameof(DeletedAt));
            }
        }

        public ObservableCollection<FootballEventTag> FootballEventTags { get; set; } = new ObservableCollection<FootballEventTag>();
        public override string ToString()
        {
            return Name;
        }
    }
}
