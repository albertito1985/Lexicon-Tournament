using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs
{
    public record GameUpdateDTO : GameBaseDTO
    {
        public int TournamentDetailsId { get; set; }
    }
}
