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
using Tournament.Core.Request;

namespace Tournament.Data.Repositories
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

        public async Task<PagedList<Game>> GetAllAsync(GameGetParamsDTO getParams, bool trackChanges = false)
        {
            IQueryable<Game> query = context.Game;
            
            if (trackChanges) query.AsNoTracking();
            
            var TotalItems = query.Count();
            
            if (getParams.StartTime!= null) query = query.Where(g => g.Time > getParams.StartTime);
            if (getParams.EndTime!= null) query = query.Where(g => g.Time < getParams.EndTime);
            if (getParams.OrderCriteria != null) query = query.OrderBy(getParams.OrderCriteria);

            return await PagedList<Game>.CreateAsync(query, getParams.PageNumber, getParams.PageSize);
        }

        public async Task<Game> GetAsync(int id)
        {
            return await context.Game.FindAsync(id);
        }

        public async Task<Game> GetAsync(string title)
        {
            return await context.Game.FirstAsync(g => g.Title == title);
        }

        public void Remove(Game game)
        {
            context.Game.Remove(game);
        }

        public void Update(Game game)
        {
            context.Entry(game).State = EntityState.Modified;
        }

        public async Task<int> GetTournamentsGamesCount(int tournamentId)
        {
            var games = await context.Game.Where(g => g.TournamentDetailsId == tournamentId).AsNoTracking().ToListAsync();
            return games.Count;
        }
    }
}
