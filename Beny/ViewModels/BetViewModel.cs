using Beny.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Beny.ViewModels
{
    public class BetViewModel : ViewModel
    {
        public List<string> TeamList { get; set; }
        public BetViewModel() {
            TeamList = ["Ливерпуль", "Челси", "Спартак", "Зенит", "Ахмат"];

            
        }
    }
}
