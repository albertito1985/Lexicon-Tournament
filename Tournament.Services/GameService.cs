using AutoMapper;
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
        //TODO: Aquí me quedé
        public async Task<(IEnumerable<GameDTO> gamesDTO, RequestMetaData metaData)> GetGame(GameGetParamsDTO getparamsDTO, bool trackChanges)
        {
            var GamesPagedList = await uow.GameRepository.GetAllAsync(getparamsDTO,trackChanges);

            var CollectionDTOofGamesDTO = mapper.Map<IEnumerable<GameDTO>>(GamesPagedList.Items);

            return (CollectionDTOofGamesDTO, GamesPagedList.MetaData);            
        }

        public async Task<GameDTO> GetGameFromId(int id)
        {
            ArgumentNullException.ThrowIfNull(id);

            var game = await uow.GameRepository.GetAsync(id) ?? throw new ArgumentException($"There is no game with id {id}");

            var gameDTO = mapper.Map<GameDTO>(game);
            return gameDTO;
        }

        public async Task<GameDTO> GetGameFromTitle(string title)
        {
            ArgumentNullException.ThrowIfNull(title);
            var game = await uow.GameRepository.GetAsync(title); //?? throw new ArgumentException($"There is no game with title {title}");
            var gameDTO = mapper.Map<GameDTO>(game);
            return gameDTO;
        }

        public async Task PutGame(int id, GameUpdateDTO gameDTO)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(gameDTO);

            var game = await uow.GameRepository.GetAsync(id) ?? throw new ArgumentException($"There is no game with id {id}");
            mapper.Map(gameDTO, game);
            uow.GameRepository.Update(game);

            await uow.PersistAsync();
        }

        public async Task<int> PostGame(GameUpdateDTO gameDTO)
        {
            ArgumentNullException.ThrowIfNull(gameDTO);

            if (await uow.GameRepository.GetTournamentsGamesCount(gameDTO.TournamentDetailsId) >= 10)
                throw new ArgumentException("A tournament cannot have more than 10 games.");

            var game = mapper.Map<Game>(gameDTO);

            uow.GameRepository.Add(game);
            await uow.PersistAsync();

            //gameDTO = mapper.Map<GameUpdateDTO>(game);

            //return CreatedAtAction("GetGame", new { id = game.Id }, gameDTO);
            return game.Id;
        }

        public async Task DeleteGame(int id)
        {
            var game = await uow.GameRepository.GetAsync(id) ?? throw new ArgumentException($"There is no game with id {id}");
            uow.GameRepository.Remove(game);
            await uow.PersistAsync();
        }

        public async Task PatchGame(int id, JsonPatchDocument<GameUpdateDTO> patchDoc)
        {
            if (patchDoc == null) throw new ArgumentException("There is an error with the changes you want to make");

            var gameToPatch = await uow.GameRepository.GetAsync(id) ?? throw new ArgumentException($"There is no game with id {id}");
            var dto = mapper.Map<GameUpdateDTO>(gameToPatch);

            //patchDoc.ApplyTo(dto, ModelState);

            TryValidateModel(dto);

            if (!ModelState.IsValid)
            {
                return;
            }

            mapper.Map(dto, gameToPatch);
            await uow.PersistAsync();
        }

        //No calls to GameExists
        private async Task<bool> GameExists(int id)
        {
            return await uow.GameRepository.AnyAsync(id);
        }
    }


}
