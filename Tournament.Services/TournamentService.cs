﻿using AutoMapper;
using Domain.Models.Exceptions;
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
using Tournament.Core.Request;

namespace Tournament.Services
{
    public class TournamentService(IUnitOfWork uow, IMapper mapper) : ControllerBase, ITournamentService
    {
        public async Task<(IEnumerable<TournamentDetailsDTO> tournamentDetailsDTOs, RequestMetaData metaData)> GetTournamentDetails(TournamentGetParamsDTO getParams, bool trackChanges)
        {
            var tournamentsPagedList = await uow.TournamentRepository.GetAllAsync(getParams, trackChanges);
            var tournametnsDTOs = mapper.Map<IEnumerable<TournamentDetailsDTO>>(tournamentsPagedList.Items);
            return (tournametnsDTOs, tournamentsPagedList.MetaData);
        }

        public async Task<TournamentDetailsDTO> GetTournamentDetails(int id)
        {
            var tournamentDetails = await uow.TournamentRepository.GetAsync(id) ?? throw new TournamentNotFoundException(id);
            var tournamentDetailsDTO = mapper.Map<TournamentDetailsDTO>(tournamentDetails);

            return tournamentDetailsDTO;
        }

        public async Task PutTournamentDetails(int id, TournamentUpdateDTO tournamentDTO)
        {
            if (tournamentDTO.Games.Count>10)
                throw new TournamentBadRequestException("A tournament cannot have more than 10 games.");

            var tournament = await uow.TournamentRepository.GetAsync(id) ?? throw new TournamentNotFoundException(id);
            mapper.Map(tournamentDTO, tournament);
            uow.TournamentRepository.Update(tournament);
            
            await uow.PersistAsync();
        }

        public async Task<int> PostTournamentDetails(TournamentDetailsDTO tournamentDetailsDTO)
        {
            if (tournamentDetailsDTO.Games.Count>10)
                throw new TournamentBadRequestException("A Tournament cannot have more than 10 games.");

            var tournamentDetails = mapper.Map<TournamentDetails>(tournamentDetailsDTO);
            uow.TournamentRepository.Add(tournamentDetails);
            await uow.PersistAsync();

            return tournamentDetails.Id;
        }

        public async Task DeleteTournamentDetails(int id)
        {
            var tournamentDetails = await uow.TournamentRepository.GetAsync(id) ?? throw new TournamentNotFoundException(id);
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
            var tournamentToPatch = await uow.TournamentRepository.GetAsync(id) ?? throw new TournamentNotFoundException(id);
            var dto = mapper.Map<TournamentUpdateDTO>(tournamentToPatch);

            patchDoc.ApplyTo(dto);
            if (dto.Games.Count>10) throw new TournamentBadRequestException("A tournament cannot have more than 10 games.");

            //TODO: fixa senare
            //TryValidateModel(dto);

            if (!ModelState.IsValid)
            {
                throw new TournamentBadRequestException("There is an error with the new data input.");
            }
            mapper.Map(dto, tournamentToPatch);
            await uow.PersistAsync();
        }
        //private async Task<bool> TournamentDetailsExists(int id)
        //{
        //    return await uow.TournamentRepository.AnyAsync(id);
        //}
    }
}
