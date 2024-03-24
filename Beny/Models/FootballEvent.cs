using Beny.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beny.Models
{
    public class FootballEvent
    {
        public int Id { get; set; }
        public Bet Bet { get; set; }
        public Team HomeTeam { get; set; }
        public Team GuestTeam { get; set; }
        public Competition Competition { get; set; }
        public float Coefficient { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CreatedAt { get;set; }
        public string Forecast {  get; set; }
        public FootballEventStatus FootballEventStatus { get; set; }
    }
}
