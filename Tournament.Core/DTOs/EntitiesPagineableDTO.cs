using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs
{
    public record EntitiesPagineableDTO
    {
        private int _pageSize = 20;
        public int PageNumber { get; set; } = 1;
        public int PageSize {
            get =>_pageSize;
            set => _pageSize = value > 100? 100 : value;
        }
    }
}
