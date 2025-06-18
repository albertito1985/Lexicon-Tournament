using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Entities
{
    public  class TournamentDetails
    {
        public int Id { get; set; }
        public required string Ttile { get; set; }
        public DateTime StartDate { get; set; }
        public required ICollection<Game> Games { get; set; }
    }
}
