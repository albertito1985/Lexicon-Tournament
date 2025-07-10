using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Request
{
    public class RequestMetaData
    {
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }

        public RequestMetaData(int totalPages, int pageSize, int currentPage, int totalItems)
        {
            TotalPages = totalPages;
            PageSize  = pageSize;
            CurrentPage = currentPage;
            TotalItems = totalItems;
        }
    }
}
