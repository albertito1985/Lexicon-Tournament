using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Turnament.Data.Repositories
{
    public class UnitOfWork(TournamentContext context, ITournamentRepository tournamentRepository, IGameRepository gameRepository) : IUnitOfWork
    {
        public ITournamentRepository TournamentRepository => tournamentRepository;

        public IGameRepository GameRepository => gameRepository;

        public async Task PersistAsync()
        {
            await context.SaveChangesAsync();
        }
    }

}
