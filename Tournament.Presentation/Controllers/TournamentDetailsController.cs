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
            var pagedResult = await serviceManager.TournamentService.GetTournamentDetails(getParams, false);
            Response.Headers["X-Pagination"] = System.Text.Json.JsonSerializer.Serialize(pagedResult.metaData);
            return Ok(pagedResult.tournamentDetailsDTOs);
        }

        // GET: api/TournamentDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDetailsDTO>> GetTournamentDetails(int id)
        {
            return Ok(await serviceManager.TournamentService.GetTournamentDetails(id));
        }

        // PUT: api/TournamentDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournamentDetails(int id, TournamentUpdateDTO tournamentDTO)
        {
            await serviceManager.TournamentService.PutTournamentDetails(id, tournamentDTO);
            return NoContent();
        }

        // POST: api/TournamentDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TournamentDetails>> PostTournamentDetails(TournamentDetailsDTO tournamentDetailsDTO)
        {
            int tournamentDetailsId = await serviceManager.TournamentService.PostTournamentDetails(tournamentDetailsDTO);
            return CreatedAtAction("GetTournamentDetails", new { id = tournamentDetailsId }, tournamentDetailsDTO);
        }

        // DELETE: api/TournamentDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournamentDetails(int id)
        {
            await serviceManager.TournamentService.DeleteTournamentDetails(id);
           
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchTournament(int id, JsonPatchDocument<TournamentUpdateDTO> patchDoc)
        {
            await serviceManager.TournamentService.PatchTournament(id, patchDoc);
           
            return NoContent();

        }
    }
}
