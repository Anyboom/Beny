using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Beny.Models
{
    public class Team
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
