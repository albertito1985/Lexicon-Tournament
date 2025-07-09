using Microsoft.AspNetCore.JsonPatch;
using Tournament.Core.DTOs;
using Tournament.Core.Request;

namespace Service.Contracts
{
    public interface IGameService
    {
        Task DeleteGame(int id);
        Task<(IEnumerable<GameDTO> gamesDTO, RequestMetaData metaData)> GetGame(GameGetParamsDTO getparamsDTO, bool trackChanges);
        Task<GameDTO> GetGameFromId(int id);
        Task<GameDTO> GetGameFromTitle(string title);
        Task PatchGame(int id, JsonPatchDocument<GameUpdateDTO> patchDoc);
        Task<int> PostGame(GameUpdateDTO gameDTO);
        Task PutGame(int id, GameUpdateDTO gameDTO);
    }
}