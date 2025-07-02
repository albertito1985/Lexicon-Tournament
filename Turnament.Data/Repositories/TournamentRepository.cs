using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using System.Linq.Dynamic.Core;

namespace Tournament.Data.Repositories
{
    public class TournamentRepository(TournamentContext context) : ITournamentRepository
    {
        public void Add(TournamentDetails tournament)
        {
            context.TournamentDetails.Add(tournament);
        }

        public async Task<bool> AnyAsync(int id)
        {
            return await context.TournamentDetails.AnyAsync(t => t.Id == id);
        }

        public async Task<CollectionResponseDTO<TournamentDetails>> GetAllAsync(TournamentGetParamsDTO getParams)
        {

            IQueryable<TournamentDetails> query = context.TournamentDetails;

            var TotalItems = query.Count();
            if (getParams.IncludeGames == true) query = query.Include(t => t.Games);
            if (getParams.StartDate != null) query = query.Where(t => t.StartDate >= getParams.StartDate);
            if (getParams.EndDate != null) query = query.Where(t => t.StartDate <= getParams.EndDate);
            if (getParams.OrderCriteria != null) query = query.OrderBy(getParams.OrderCriteria);

            int TotalPages = (int)Math.Ceiling((double)TotalItems / getParams.PageSize);
            if (getParams.PageSize>100) getParams.PageSize = 100;
            int skip = (getParams.PageNumber - 1) * getParams.PageSize;
            query = query.Skip(skip).Take(getParams.PageSize);
            var items = await query.ToListAsync();

            return new CollectionResponseDTO<TournamentDetails>()
            {
                Items = items,
                TotalPages = TotalPages,
                PageSize = items.Count,
                CurrentPage = getParams.PageNumber,
                TotalItems = TotalItems
            };
        }

        public async Task<TournamentDetails> GetAsync(int id)
        {

            return await context.TournamentDetails
                .Include(t => t.Games)
                .FirstAsync(t => t.Id == id);
        }

        public void Remove(TournamentDetails tournament)
        {
            context.TournamentDetails.Remove(tournament);
        }

        public void Update(TournamentDetails tournament)
        {
            context.Entry(tournament).State = EntityState.Modified;
        }
    }
}
