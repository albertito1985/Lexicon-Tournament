using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs
{
    public class CollectionResponseDTO<T> where T: class
    {
        public int TotalPages { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public int CurrentPage { get; set; } = 0;
        public int TotalItems { get; set; } = 0;
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }
}
