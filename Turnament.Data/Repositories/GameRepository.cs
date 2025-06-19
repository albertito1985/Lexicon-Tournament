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
    public class GameRepository(TournamentContext context) : IGameRepository
    {
        public void Add(Game game)
        {
            context.Game.Add(game);
        }

        public async Task<bool> AnyAsync(int id)
        {
            return await context.Game.AnyAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await context.Game.ToListAsync();
        }

        public async Task<Game> GetAsync(int id)
        {
            return await context.Game.FindAsync(id);
        }

        public void Remove(Game game)
        {
            context.Game.Remove(game);
        }

        public void Update(Game game)
        {
            context.Entry(game).State = EntityState.Modified;
        }
    }
}
