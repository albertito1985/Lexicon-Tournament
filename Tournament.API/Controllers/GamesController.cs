using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController(IUnitOfWork UOW, IMapper mapper) : ControllerBase
    {

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGame([FromQuery] GameGetParamsDTO getparamsDTO)
        {
            var games = await UOW.GameRepository.GetAllAsync(getparamsDTO);
            var gameDTOs = mapper.Map<IEnumerable<Game>>(games);
            return Ok(gameDTOs);
        }

        // GET: api/Games/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Game>> GetGameFromId(int id)
        {
            var game = await UOW.GameRepository.GetAsync(id);

            if (game == null)
            {
                return NotFound();
            }
            var gameDTO = mapper.Map<Game>(game);
            return Ok(gameDTO);
        }

        // GET: api/Games/5
        [HttpGet("{title}")]
        public async Task<ActionResult<Game>> GetGameFromTitle(string title)
        {
            var game = await UOW.GameRepository.GetAsync(title);

            if (game == null)
            {
                return NotFound();
            }
            var gameDTO = mapper.Map<Game>(game);
            return Ok(gameDTO);
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameUpdateDTO gameDTO)
        {
            var game = await UOW.GameRepository.GetAsync(id);
            if (game == null)
            {
                return BadRequest();
            }

            mapper.Map(gameDTO, game);
            UOW.GameRepository.Update(game);

            try
            {
                await UOW.PersistAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(GameUpdateDTO gameDTO)
        {
            if (gameDTO == null)
            {
                return BadRequest("Game cannot be null.");
            }

            var game = mapper.Map<Game>(gameDTO);

            UOW.GameRepository.Add(game);
            await UOW.PersistAsync();

            gameDTO = mapper.Map<GameUpdateDTO>(game);

            return CreatedAtAction("GetGame", new { id = game.Id }, gameDTO);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await UOW.GameRepository.GetAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            UOW.GameRepository.Remove(game);
            await UOW.PersistAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchGame(int id, JsonPatchDocument<GameUpdateDTO> patchDoc)
        {
            if (patchDoc == null) return BadRequest("no patch document");

            var gameToPatch = await UOW.GameRepository.GetAsync(id);

            if (gameToPatch.Equals(null)) return NotFound("Game does not exist");

            var dto = mapper.Map<GameUpdateDTO>(gameToPatch);

            patchDoc.ApplyTo(dto, ModelState);

            TryValidateModel(dto);

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            mapper.Map(dto, gameToPatch);
            await UOW.PersistAsync();

            return NoContent();

        }

        private async Task<bool> GameExists(int id)
        {
            return await UOW.GameRepository.AnyAsync(id);
        }
    }
}
