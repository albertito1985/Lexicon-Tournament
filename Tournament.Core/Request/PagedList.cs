using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Core.Request
{
    public class PagedList<T> where T : class
    {
        public IReadOnlyList<T> Items { get; init; }
        public RequestMetaData MetaData { get; set; }

        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            Items = new List<T>(items).AsReadOnly();
            MetaData = new RequestMetaData(
                totalItems : count,
                pageSize : pageSize,
                currentPage: pageNumber,
                totalPages : (int) Math.Ceiling(count / (double) pageSize)
            );
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
