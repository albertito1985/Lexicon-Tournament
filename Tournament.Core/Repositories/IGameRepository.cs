using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Request;

namespace Tournament.Core.Repositories
{
    public interface IGameRepository
    {
        Task<PagedList<Game>> GetAllAsync(GameGetParamsDTO getParams, bool trackChanges = false);
        Task<Game> GetAsync(int id);
        Task<Game> GetAsync(string title);
        Task<bool> AnyAsync(int id);
        void Add(Game game);
        void Update(Game game);
        void Remove(Game game);
        Task<int> GetTournamentsGamesCount(int tournamentId);
    }
}
