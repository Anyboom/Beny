using Beny.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beny.Models
{
    public class Sport : BindableBase
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
                OnPropertyChanged(nameof(Id));
                _Id = value;
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnPropertyChanged(nameof(Name));
                _Name = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
