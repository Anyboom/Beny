using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beny.Models.Interfaces
{
    public interface IDictionaryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
