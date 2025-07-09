using Microsoft.AspNetCore.JsonPatch;
using Tournament.Core.DTOs;
using Tournament.Core.Request;

namespace Service.Contracts
{
    public interface ITournamentService
    {
        Task DeleteTournamentDetails(int id);
        Task<TournamentDetailsDTO> GetTournamentDetails(int id);
        Task<(IEnumerable<TournamentDetailsDTO> tournamentDetailsDTOs, RequestMetaData metaData)> GetTournamentDetails(TournamentGetParamsDTO getParams, bool trackChanges);
        Task PatchTournament(int id, JsonPatchDocument<TournamentUpdateDTO> patchDoc);
        Task<int> PostTournamentDetails(TournamentDetailsDTO tournamentDetailsDTO);
        Task PutTournamentDetails(int id, TournamentUpdateDTO tournamentDTO);
    }
}