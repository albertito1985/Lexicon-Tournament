using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs
{
    public record GameBaseDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Maximum length for the Title is 50 characters")]
        public required string Title { get; set; }
        public DateTime Time { get; set; }
    }
}
