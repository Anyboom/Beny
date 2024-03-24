using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beny.Models
{
    public class Bet
    {
        public int Id { get; set; }
        public List<FootballEvent> FootballEvents { get; set; } = new List<FootballEvent>();

        public DateTime CreatedAt { get; set; }
    }
}
