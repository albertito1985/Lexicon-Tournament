using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs
{
    public record TournamentGetParamsDTO : EntitiesPagineableDTO
    {
        public bool IncludeGames { get; set; } = false;
        public string? OrderCriteria { get; set; } = null;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
    }
}
