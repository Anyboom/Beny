using Beny.Base;
using Beny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmDialogs;

namespace Beny.ViewModels
{
    public class ShowBetViewModel : BindableBase
    {
        public Bet CurrentBet { get; set; }
    }
}
