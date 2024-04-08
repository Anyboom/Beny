using Beny.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beny.Models
{
    public class Sport : BindableBase
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
                OnPropertyChanged(nameof(Id));
                _id = value;
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
                OnPropertyChanged(nameof(Name));
                _name = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
