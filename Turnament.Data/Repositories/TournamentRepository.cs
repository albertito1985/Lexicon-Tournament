using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Turnament.Data.Repositories
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

        public async Task<IEnumerable<TournamentDetails>> GetAllAsync(bool includeEmployees = false)
        {
            return includeEmployees ? await context.TournamentDetails.Include(t =>t.Games ).ToListAsync()
                                    : await context.TournamentDetails.ToListAsync();
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
