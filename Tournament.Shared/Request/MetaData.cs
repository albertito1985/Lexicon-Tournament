using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Request
{
    public class MetaData
    {
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }

        // TODO: delete later
        //public IEnumerable<T> Items { get; set; } = new List<T>();

        public MetaData(int totalPages, int pageSize, int currentPage, int totalItems)
        {
            TotalPages = totalPages;
            PageSize  = pageSize;
            CurrentPage = currentPage;
            TotalItems = totalItems;
        }
    }
}
