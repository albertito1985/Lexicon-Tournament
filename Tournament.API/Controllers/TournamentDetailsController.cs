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

namespace Tournament.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentDetailsController(TournamentRepository repository) : ControllerBase
    {
        // GET: api/TournamentDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDetails>>> GetTournamentDetails()
        {
            return Ok(await repository.GetAllAsync());
        }

        // GET: api/TournamentDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDetails>> GetTournamentDetails(int id)
        {
            var tournamentDetails = await repository.GetAsync(id);

            if (tournamentDetails == null)
            {
                return NotFound();
            }

            return tournamentDetails;
        }

        // PUT: api/TournamentDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournamentDetails(int id, TournamentDetails tournamentDetails)
        {
            if (id != tournamentDetails.Id)
            {
                return BadRequest();
            }

            repository.Update(tournamentDetails);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TournamentDetailsExists(id))
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

        // POST: api/TournamentDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TournamentDetails>> PostTournamentDetails(TournamentDetails tournamentDetails)
        {
            repository.Add(tournamentDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTournamentDetails", new { id = tournamentDetails.Id }, tournamentDetails);
        }

        // DELETE: api/TournamentDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournamentDetails(int id)
        {
            //man ska ta bort games som har det här tournament först.
            var tournamentDetails = await repository.GetAsync(id);
            if (tournamentDetails == null)
            {
                return NotFound();
            }

            repository.Remove(tournamentDetails);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> TournamentDetailsExists(int id)
        {
            return await repository.AnyAsync(id) ;
        }
    }
}
