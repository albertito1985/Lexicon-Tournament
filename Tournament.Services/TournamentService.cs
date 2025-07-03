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
            ArgumentNullException.ThrowIfNull(id);

            var tournamentDetails = await uow.TournamentRepository.GetAsync(id) ?? throw new ArgumentException($"There is no tournament with id {id}");
            var tournamentDetailsDTO = mapper.Map<TournamentDetailsDTO>(tournamentDetails);

            return tournamentDetailsDTO;
        }

        public async Task PutTournamentDetails(int id, TournamentUpdateDTO tournamentDTO)
        {
            ArgumentNullException.ThrowIfNull(id);

            if (tournamentDTO.Games.Count>10)
                throw new ArgumentException("A tournament cannot have more than 10 games.");

            var tournament = await uow.TournamentRepository.GetAsync(id) ?? throw new ArgumentException($"There is no tournament with id {id}");
            mapper.Map(tournamentDTO, tournament);
            uow.TournamentRepository.Update(tournament);
            
            await uow.PersistAsync();
        }

        public async Task<int> PostTournamentDetails(TournamentDetailsDTO tournamentDetailsDTO)
        {
            ArgumentNullException.ThrowIfNull(tournamentDetailsDTO);

            if (tournamentDetailsDTO.Games.Count>10)
                throw new ArgumentException("A Tournament cannot have more than 10 games.");

            var tournamentDetails = mapper.Map<TournamentDetails>(tournamentDetailsDTO);
            uow.TournamentRepository.Add(tournamentDetails);
            await uow.PersistAsync();

            //send tournamentDetailsDTO maybe?
            //tournamentDetailsDTO = mapper.Map<TournamentDetailsDTO>(tournamentDetails);

            return tournamentDetails.Id;
        }

        public async Task DeleteTournamentDetails(int id)
        {
            ArgumentNullException.ThrowIfNull(id);

            var tournamentDetails = await uow.TournamentRepository.GetAsync(id) ?? throw new ArgumentException($"There is no tournament with id {id}");
            if (tournamentDetails.Games.Count > 0)
            {
                foreach (Game game in tournamentDetails.Games)
                {
                    uow.GameRepository.Remove(game);
                }
            }

            uow.TournamentRepository.Remove(tournamentDetails);
            await uow.PersistAsync();
        }

        public async Task PatchTournament(int id, JsonPatchDocument<TournamentUpdateDTO> patchDoc)
        {
            ArgumentNullException.ThrowIfNull(patchDoc);
            ArgumentNullException.ThrowIfNull(id);

            var tournamentToPatch = await uow.TournamentRepository.GetAsync(id) ?? throw new ArgumentException($"There is no tournament with id {id}");
            var dto = mapper.Map<TournamentUpdateDTO>(tournamentToPatch);

            //patchDoc.ApplyTo(dto, ModelState);
            if (dto.Games.Count>10) throw new ArgumentException("A tournament cannot have more than 10 games.");

            TryValidateModel(dto);

            if (!ModelState.IsValid)
            {
                //return UnprocessableEntity(ModelState);
            }

            //tournamentToPatch = mapper.Map<TournamentDetails>(dto);
            mapper.Map(dto, tournamentToPatch);
            await uow.PersistAsync();
        }
        private async Task<bool> TournamentDetailsExists(int id)
        {
            return await uow.TournamentRepository.AnyAsync(id);
        }
    }
}
