using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Data.Data;
using Tournament.Core.Entities;
using Turnament.Data.Repositories;
using Tournament.Core.Repositories;
using AutoMapper;
using Tournament.Core.DTOs;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Service.Contracts;

namespace Tournament.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentDetailsController(IServiceManager serviceManager) : ControllerBase
    {

        // GET: api/TournamentDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDetailsDTO>>> GetTournamentDetails([FromQuery]TournamentGetParamsDTO getParams)
        {
            //var tournaments = await UOW.TournamentRepository.GetAllAsync(getParams);
            //var tournametnsDTOs = mapper.Map<IEnumerable<TournamentDetailsDTO>>(tournaments);
            //return Ok(tournametnsDTOs);
            return Ok(await serviceManager.TournamentService.GetTournamentDetails(getParams));
        }

        // GET: api/TournamentDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDetailsDTO>> GetTournamentDetails(int id)
        {
            //var tournamentDetails = await UOW.TournamentRepository.GetAsync(id);

            //if (tournamentDetails == null)
            //{
            //    return NotFound();
            //}

            //var tournamentDetailsDTO = mapper.Map<TournamentDetailsDTO>(tournamentDetails);

            //return Ok(tournamentDetailsDTO);
            return Ok(await serviceManager.TournamentService.GetTournamentDetails(id));
        }

        // PUT: api/TournamentDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournamentDetails(int id, TournamentUpdateDTO tournamentDTO)
        {
            //var tournament = await UOW.TournamentRepository.GetAsync(id);
            //if (tournament == null)
            //{
            //    return BadRequest();
            //}

            //mapper.Map(tournamentDTO, tournament);
            //UOW.TournamentRepository.Update(tournament);

            //try
            //{
            //    await UOW.PersistAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!await TournamentDetailsExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();
            try
            {
                await serviceManager.TournamentService.PutTournamentDetails(id, tournamentDTO);
            }
            catch
            {

            }
            return NoContent();
        }

        // POST: api/TournamentDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TournamentDetails>> PostTournamentDetails(TournamentDetailsDTO tournamentDetailsDTO)
        {
            //if (tournamentDetailsDTO == null)
            //{
            //    return BadRequest("Tournament cannot be null.");
            //}

            //var tournamentDetails = mapper.Map<TournamentDetails>(tournamentDetailsDTO);
            //UOW.TournamentRepository.Add(tournamentDetails);
            //await UOW.PersistAsync();

            //tournamentDetailsDTO = mapper.Map<TournamentDetailsDTO>(tournamentDetails);

            //return CreatedAtAction("GetTournamentDetails", new { id = tournamentDetails.Id }, tournamentDetailsDTO);
            int tournamentDetailsId = await serviceManager.TournamentService.PostTournamentDetails(tournamentDetailsDTO);
            return CreatedAtAction("GetTournamentDetails", new { id = tournamentDetailsId }, tournamentDetailsDTO);
        }

        // DELETE: api/TournamentDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournamentDetails(int id)
        {
            ////man ska ta bort games som har det här tournament först.
            //var tournamentDetails = await UOW.TournamentRepository.GetAsync(id);
            //if (tournamentDetails == null)
            //{
            //    return NotFound();
            //}
            //if (tournamentDetails.Games.Count > 0)
            //{
            //    foreach(Game game in tournamentDetails.Games)
            //    {
            //        UOW.GameRepository.Remove(game);
            //    }
            //}

            //UOW.TournamentRepository.Remove(tournamentDetails);
            //await UOW.PersistAsync();

            //return NoContent();
            try
            {
                await serviceManager.TournamentService.DeleteTournamentDetails(id);
            }
            catch
            {

            }
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchTournament(int id, JsonPatchDocument<TournamentUpdateDTO> patchDoc)
        {
            //if (patchDoc is null) return BadRequest("no patch document");

            //var tournamentToPatch = await UOW.TournamentRepository.GetAsync(id);

            //if (tournamentToPatch.Equals(null)) return NotFound("Tournament does not exist");

            //var dto = mapper.Map<TournamentUpdateDTO>(tournamentToPatch);

            ////patchDoc.ApplyTo(dto, ModelState);

            //TryValidateModel(dto);

            //if (!ModelState.IsValid)
            //{
            //    return UnprocessableEntity(ModelState);
            //}

            ////tournamentToPatch = mapper.Map<TournamentDetails>(dto);
            //mapper.Map(dto, tournamentToPatch);
            //await UOW.PersistAsync();

            //return NoContent();
            try
            {
                await serviceManager.TournamentService.PatchTournament(id, patchDoc);
            }
            catch
            {

            }
            return NoContent();

        }
    }
}
