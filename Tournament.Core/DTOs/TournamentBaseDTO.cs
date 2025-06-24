using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;

namespace Tournament.Core.DTOs
{
    public record TournamentBaseDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Maximum length for the Title is 50 characters")]
        public required string Title { get; set; }
        public DateTime StartDate { get; set; }
        public ICollection<Game> Games { get; set; }
    }
}
