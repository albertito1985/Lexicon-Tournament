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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Tournament.Services
{
    public class GameService(IUnitOfWork uow, IMapper mapper) : ControllerBase, IGameService
    {
        public async Task<CollectionResponseDTO<GameDTO>> GetGame(GameGetParamsDTO getparamsDTO)
        {
            if (getparamsDTO.PageSize>100) getparamsDTO.PageSize = 100;

            var CollectionDTOofGames = await uow.GameRepository.GetAllAsync(getparamsDTO);

            var CollectionDTOofGamesDTO = mapper.Map<CollectionResponseDTO<GameDTO>>(CollectionDTOofGames);

            return CollectionDTOofGamesDTO;
        }

        public async Task<GameDTO> GetGameFromId(int id)
        {
            var game = await uow.GameRepository.GetAsync(id);

            if (game == null)
            {
                return null;
            }
            var gameDTO = mapper.Map<GameDTO>(game);
            return gameDTO;
        }

        public async Task<GameDTO> GetGameFromTitle(string title)
        {
            var game = await uow.GameRepository.GetAsync(title);

            if (game == null)
            {
                return null;
            }
            var gameDTO = mapper.Map<GameDTO>(game);
            return gameDTO;
        }

        public async Task PutGame(int id, GameUpdateDTO gameDTO)
        {
            var game = await uow.GameRepository.GetAsync(id);
            if (game == null)
            {
                return;
            }

            mapper.Map(gameDTO, game);
            uow.GameRepository.Update(game);

            try
            {
                await uow.PersistAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return;
        }

        public async Task<int> PostGame(GameUpdateDTO gameDTO)
        {
            if (gameDTO == null)
            {
                //return;
            }

            var game = mapper.Map<Game>(gameDTO);

            uow.GameRepository.Add(game);
            await uow.PersistAsync();

            //gameDTO = mapper.Map<GameUpdateDTO>(game);

            //return CreatedAtAction("GetGame", new { id = game.Id }, gameDTO);
            return game.Id;
        }

        public async Task DeleteGame(int id)
        {
            var game = await uow.GameRepository.GetAsync(id);
            if (game == null)
            {
                //Need to make a differece between not found and already deleted
                return;
            }

            uow.GameRepository.Remove(game);
            await uow.PersistAsync();
        }

        public async Task PatchGame(int id, JsonPatchDocument<GameUpdateDTO> patchDoc)
        {
            if (patchDoc == null) return;

            var gameToPatch = await uow.GameRepository.GetAsync(id);

            if (gameToPatch== null) return;

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
