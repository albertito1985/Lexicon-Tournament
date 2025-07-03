using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
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
            //var games = await UOW.GameRepository.GetAllAsync(getparamsDTO);
            //var gameDTOs = mapper.Map<IEnumerable<GameDTO>>(games);
            //return Ok(gameDTOs);
            return Ok(await serviceManager.GameService.GetGame(getparamsDTO));
        }

        // GET: api/Games/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GameDTO>> GetGameFromId(int id)
        {
            //var game = await UOW.GameRepository.GetAsync(id);

            //if (game == null)
            //{
            //    return NotFound();
            //}
            //var gameDTO = mapper.Map<GameDTO>(game);
            //return Ok(gameDTO);
            return Ok(await serviceManager.GameService.GetGameFromId(id));
        }

        // GET: api/Games/5
        [HttpGet("{title}")]
        public async Task<ActionResult<Game>> GetGameFromTitle(string title)
        {
            //var game = await UOW.GameRepository.GetAsync(title);

            //if (game == null)
            //{
            //    return NotFound();
            //}
            //var gameDTO = mapper.Map<GameDTO>(game);
            //return Ok(gameDTO);
            return Ok(await serviceManager.GameService.GetGameFromTitle(title));
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameUpdateDTO gameDTO)
        {
            //var game = await UOW.GameRepository.GetAsync(id);
            //if (game == null)
            //{
            //    return NotFound();
            //}

            //mapper.Map(gameDTO, game);
            //UOW.GameRepository.Update(game);

            //try
            //{
            //    await UOW.PersistAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    throw;
            //}

            //return NoContent();

            await serviceManager.GameService.PutGame(id, gameDTO);

            return NoContent();
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(GameUpdateDTO gameDTO)
        {
            //if (gameDTO == null)
            //{
            //    return BadRequest("Game cannot be null.");
            //}

            //var game = mapper.Map<Game>(gameDTO);

            //UOW.GameRepository.Add(game);
            //await UOW.PersistAsync();

            //return CreatedAtAction("GetGame", new { id = game.Id }, gameDTO);
            int gameId= await serviceManager.GameService.PostGame(gameDTO);
            return CreatedAtAction("GetGame", new { id = gameId }, gameDTO);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            //var game = await UOW.GameRepository.GetAsync(id);
            //if (game == null)
            //{
            //    return NotFound();
            //}

            //UOW.GameRepository.Remove(game);
            //await UOW.PersistAsync();

            //return NoContent();

            await serviceManager.GameService.DeleteGame(id);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchGame(int id, JsonPatchDocument<GameUpdateDTO> patchDoc)
        {
            //if (patchDoc == null) return BadRequest("no patch document");

            //var gameToPatch = await UOW.GameRepository.GetAsync(id);

            //if (gameToPatch== null) return NotFound("Game does not exist");

            //var dto = mapper.Map<GameUpdateDTO>(gameToPatch);

            ////patchDoc.ApplyTo(dto, ModelState);

            //TryValidateModel(dto);

            //if (!ModelState.IsValid)
            //{
            //    return UnprocessableEntity(ModelState);
            //}

            //mapper.Map(dto, gameToPatch);
            //await UOW.PersistAsync();

            //return NoContent();

            await serviceManager.GameService.PatchGame(id, patchDoc);

            return NoContent();

        }
    }
}
