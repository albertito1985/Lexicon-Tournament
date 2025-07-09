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
    public interface ITournamentRepository
    {
        Task<PagedList<TournamentDetails>> GetAllAsync(TournamentGetParamsDTO getParams, bool trackChanges);
        Task<TournamentDetails> GetAsync(int id);
        Task<bool> AnyAsync(int id);
        void Add(TournamentDetails tournament);
        void Update(TournamentDetails tournament);
        void Remove(TournamentDetails tournament);
    }
}
