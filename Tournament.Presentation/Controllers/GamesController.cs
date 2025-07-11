using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using System.Text.Json;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;

namespace Tournament.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController(IServiceManager serviceManager) : ControllerBase
    {

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetGame([FromQuery] GameGetParamsDTO getparamsDTO)
        {
            var pagedResult = await serviceManager.GameService.GetGame(getparamsDTO, false);
            Response.Headers["X-Pagination"] = JsonSerializer.Serialize(pagedResult.metaData);
            return Ok(pagedResult.gamesDTO);
        }

        // GET: api/Games/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GameDTO>> GetGameFromId(int id)
        {
            return Ok(await serviceManager.GameService.GetGameFromId(id));
        }

        // GET: api/Games/5
        [HttpGet("{title}")]
        public async Task<ActionResult<GameDTO>> GetGameFromTitle(string title)
        {
            return Ok(await serviceManager.GameService.GetGameFromTitle(title));
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameUpdateDTO gameDTO)
        {
            await serviceManager.GameService.PutGame(id, gameDTO);

            return NoContent();
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(GameUpdateDTO gameDTO)
        {
            int gameId= await serviceManager.GameService.PostGame(gameDTO);
            return CreatedAtAction("GetGame", new { id = gameId }, gameDTO);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            await serviceManager.GameService.DeleteGame(id);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchGame(int id, JsonPatchDocument<GameUpdateDTO> patchDoc)
        {
            await serviceManager.GameService.PatchGame(id, patchDoc);

            return NoContent();

        }
    }
}
