using AutoMapper;
using Domain.Models.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Tournament.Services
{
    public class GameService(IUnitOfWork uow, IMapper mapper) : ControllerBase, IGameService
    {
        public async Task<(IEnumerable<GameDTO> gamesDTO, RequestMetaData metaData)> GetGame(GameGetParamsDTO getparamsDTO, bool trackChanges)
        {
            var GamesPagedList = await uow.GameRepository.GetAllAsync(getparamsDTO,trackChanges);

            var CollectionDTOofGamesDTO = mapper.Map<IEnumerable<GameDTO>>(GamesPagedList.Items);

            return (CollectionDTOofGamesDTO, GamesPagedList.MetaData);            
        }

        public async Task<GameDTO> GetGameFromId(int id)
        {
            var game = await uow.GameRepository.GetAsync(id) ?? throw new GameNotFoundException(id);

            var gameDTO = mapper.Map<GameDTO>(game);
            return gameDTO;
        }

        public async Task<GameDTO> GetGameFromTitle(string title)
        {
            var game = await uow.GameRepository.GetAsync(title)?? throw new GameTitleNotFoundException(title);
            var gameDTO = mapper.Map<GameDTO>(game);
            return gameDTO;
        }

        public async Task PutGame(int id, GameUpdateDTO gameDTO)
        {
            var game = await uow.GameRepository.GetAsync(id) ?? throw new GameNotFoundException(id);
            mapper.Map(gameDTO, game);
            uow.GameRepository.Update(game);

            await uow.PersistAsync();
        }

        public async Task<int> PostGame(GameUpdateDTO gameDTO)
        {
            if (await uow.GameRepository.GetTournamentsGamesCount(gameDTO.TournamentDetailsId) >= 10)
                throw new GameBadRequestException("A tournament cannot have more than 10 games.");

            var game = mapper.Map<Game>(gameDTO);

            uow.GameRepository.Add(game);
            await uow.PersistAsync();

            return game.Id;
        }

        public async Task DeleteGame(int id)
        {
            var game = await uow.GameRepository.GetAsync(id) ?? throw new GameNotFoundException(id);
            uow.GameRepository.Remove(game);
            await uow.PersistAsync();
        }

        public async Task PatchGame(int id, JsonPatchDocument<GameUpdateDTO> patchDoc)
        {
            var gameToPatch = await uow.GameRepository.GetAsync(id) ?? throw new GameNotFoundException(id);
            var dto = mapper.Map<GameUpdateDTO>(gameToPatch);

            patchDoc.ApplyTo(dto);

            // TODO: Ask teacher
            //TryValidateModel(dto);

            if (!ModelState.IsValid)
            {
                throw new GameBadRequestException("There is an error with the new data input.");
            }

            mapper.Map(dto, gameToPatch);
            await uow.PersistAsync();
        }

        //No calls to GameExists
        //private async Task<bool> GameExists(int id)
        //{
        //    return await uow.GameRepository.AnyAsync(id);
        //}
    }


}
