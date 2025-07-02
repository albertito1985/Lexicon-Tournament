using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;

namespace Tournament.Services
{
    public class TournamentService(IUnitOfWork uow, IMapper mapper) : ControllerBase, ITournamentService
    {
        public async Task<CollectionResponseDTO<TournamentDetailsDTO>> GetTournamentDetails(TournamentGetParamsDTO getParams)
        {
            var tournaments = await uow.TournamentRepository.GetAllAsync(getParams);
            var tournametnsDTOs = mapper.Map<CollectionResponseDTO<TournamentDetailsDTO>>(tournaments);
            return tournametnsDTOs;
        }

        public async Task<TournamentDetailsDTO> GetTournamentDetails(int id)
        {
            var tournamentDetails = await uow.TournamentRepository.GetAsync(id);

            if (tournamentDetails == null)
            {
                return null;
            }

            var tournamentDetailsDTO = mapper.Map<TournamentDetailsDTO>(tournamentDetails);

            return tournamentDetailsDTO;
        }

        public async Task PutTournamentDetails(int id, TournamentUpdateDTO tournamentDTO)
        {
            var tournament = await uow.TournamentRepository.GetAsync(id);
            if (tournament == null)
            {
                //return null;
            }

            mapper.Map(tournamentDTO, tournament);
            uow.TournamentRepository.Update(tournament);

            try
            {
                await uow.PersistAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TournamentDetailsExists(id))
                {
                    //return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
        }

        public async Task<int> PostTournamentDetails(TournamentDetailsDTO tournamentDetailsDTO)
        {
            if (tournamentDetailsDTO == null)
            {
                //return BadRequest("Tournament cannot be null.");
            }

            var tournamentDetails = mapper.Map<TournamentDetails>(tournamentDetailsDTO);
            uow.TournamentRepository.Add(tournamentDetails);
            await uow.PersistAsync();

            tournamentDetailsDTO = mapper.Map<TournamentDetailsDTO>(tournamentDetails);

            return tournamentDetails.Id;
        }

        public async Task DeleteTournamentDetails(int id)
        {
            //man ska ta bort games som har det här tournament först.
            var tournamentDetails = await uow.TournamentRepository.GetAsync(id);
            if (tournamentDetails == null)
            {
                //return NotFound();
            }
            if (tournamentDetails.Games.Count > 0)
            {
                foreach (Game game in tournamentDetails.Games)
                {
                    uow.GameRepository.Remove(game);
                }
            }

            uow.TournamentRepository.Remove(tournamentDetails);
            await uow.PersistAsync();

            //return NoContent();
        }

        public async Task PatchTournament(int id, JsonPatchDocument<TournamentUpdateDTO> patchDoc)
        {
            //if (patchDoc is null) return BadRequest("no patch document");

            var tournamentToPatch = await uow.TournamentRepository.GetAsync(id);

            //if (tournamentToPatch.Equals(null)) return NotFound("Tournament does not exist");

            var dto = mapper.Map<TournamentUpdateDTO>(tournamentToPatch);

            //patchDoc.ApplyTo(dto, ModelState);

            TryValidateModel(dto);

            if (!ModelState.IsValid)
            {
                //return UnprocessableEntity(ModelState);
            }

            //tournamentToPatch = mapper.Map<TournamentDetails>(dto);
            mapper.Map(dto, tournamentToPatch);
            await uow.PersistAsync();

            //return NoContent();

        }
        private async Task<bool> TournamentDetailsExists(int id)
        {
            return await uow.TournamentRepository.AnyAsync(id);
        }
    }
}
