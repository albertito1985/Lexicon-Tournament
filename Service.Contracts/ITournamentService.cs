using Microsoft.AspNetCore.JsonPatch;
using Tournament.Core.DTOs;

namespace Service.Contracts
{
    public interface ITournamentService
    {
        Task DeleteTournamentDetails(int id);
        Task<TournamentDetailsDTO> GetTournamentDetails(int id);
        Task<CollectionResponseDTO<TournamentDetailsDTO>> GetTournamentDetails(TournamentGetParamsDTO getParams);
        Task PatchTournament(int id, JsonPatchDocument<TournamentUpdateDTO> patchDoc);
        Task<int> PostTournamentDetails(TournamentDetailsDTO tournamentDetailsDTO);
        Task PutTournamentDetails(int id, TournamentUpdateDTO tournamentDTO);
    }
}