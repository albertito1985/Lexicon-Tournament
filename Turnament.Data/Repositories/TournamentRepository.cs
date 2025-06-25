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

        public async Task<IEnumerable<TournamentDetails>> GetAllAsync(TournamentGetParamsDTO getParams)
        {
            if(getParams == null) return await context.TournamentDetails.ToListAsync();

            IQueryable<TournamentDetails> query = context.TournamentDetails;

            if (getParams.IncludeGames == true) query = query.Include(t => t.Games);
            if (getParams.StartDate != null) query = query.Where(t => t.StartDate >= getParams.StartDate);
            if (getParams.EndDate != null) query = query.Where(t => t.StartDate <= getParams.EndDate);
            if (getParams.OrderCriteria != null) query = query.OrderBy(getParams.OrderCriteria);

            int skip = (getParams.PageNumber - 1) * getParams.PageSize;
            query = query.Skip(skip).Take(getParams.PageSize);

            return query.ToList();
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
