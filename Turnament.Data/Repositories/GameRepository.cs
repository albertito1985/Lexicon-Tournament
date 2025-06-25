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

        public async Task<IEnumerable<Game>> GetAllAsync(GameGetParamsDTO getParams)
        {
            if (getParams == null) return await context.Game.ToListAsync();

            IQueryable<Game> query = context.Game;

            if (getParams.StartTime!= null) query = query.Where(g => g.Time > getParams.StartTime);
            if (getParams.EndTime!= null) query = query.Where(g => g.Time < getParams.EndTime);
            if (getParams.OrderCriteria != null) query = query.OrderBy(getParams.OrderCriteria);

            int skip = (getParams.PageNumber - 1) * getParams.PageSize;
            query = query.Skip(skip).Take(getParams.PageSize);

            return await query.ToListAsync();
        }

        public async Task<Game> GetAsync(int id)
        {
            return await context.Game.FindAsync(id);
        }

        public async Task<Game> GetAsync(string title)
        {
            return await context.Game.FirstAsync(g =>g.Title == title);
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
