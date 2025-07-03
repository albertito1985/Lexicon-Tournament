using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;

namespace Tournament.Core.Repositories
{
    public interface IGameRepository
    {
        Task<CollectionResponseDTO<Game>> GetAllAsync(GameGetParamsDTO getParams);
        Task<Game> GetAsync(int id);
        Task<Game> GetAsync(string title);
        Task<bool> AnyAsync(int id);
        void Add(Game game);
        void Update(Game game);
        void Remove(Game game);
        Task<int> GetTournamentsGamesCount(int tournamentId);
    }
}
