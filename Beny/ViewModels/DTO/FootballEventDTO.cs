using Beny.Models.Enums;
using Beny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Beny.ViewModels.Base;
using System.Collections;

namespace Beny.ViewModels.DTO
{
    public class FootballEventDTO : BindableBase
    {
        public string HomeTeam { get; set; }
        public string GuestTeam { get; set; }
        public string Competition { get; set; }
        public string Coefficient { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int Hour { get; set; }
        public int Minute { get; set; }
        public string Forecast { get; set; }
        public string Sport { get; set; }
    }
}
