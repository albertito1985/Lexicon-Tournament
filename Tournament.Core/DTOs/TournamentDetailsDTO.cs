using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;

namespace Tournament.Core.DTOs
{
    public record TournamentDetailsDTO : TournamentBaseDTO
    {
        public DateTime EndDate { get; set; }
    }
}
