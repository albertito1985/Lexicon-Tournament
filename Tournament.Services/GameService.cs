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
    public class GameService(IUnitOfWork UOW, IMapper mapper) : ControllerBase, IGameService
    {
        public async Task<IEnumerable<GameDTO>> GetGame(GameGetParamsDTO getparamsDTO)
        {
            var games = await UOW.GameRepository.GetAllAsync(getparamsDTO);
            var gameDTOs = mapper.Map<IEnumerable<GameDTO>>(games);
            return gameDTOs;
        }

        public async Task<GameDTO> GetGameFromId(int id)
        {
            var game = await UOW.GameRepository.GetAsync(id);

            if (game == null)
            {
                return null;
            }
            var gameDTO = mapper.Map<GameDTO>(game);
            return gameDTO;
        }

        public async Task<GameDTO> GetGameFromTitle(string title)
        {
            var game = await UOW.GameRepository.GetAsync(title);

            if (game == null)
            {
                return null;
            }
            var gameDTO = mapper.Map<GameDTO>(game);
            return gameDTO;
        }

        public async Task PutGame(int id, GameUpdateDTO gameDTO)
        {
            var game = await UOW.GameRepository.GetAsync(id);
            if (game == null)
            {
                return;
            }

            mapper.Map(gameDTO, game);
            UOW.GameRepository.Update(game);

            try
            {
                await UOW.PersistAsync();
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

            UOW.GameRepository.Add(game);
            await UOW.PersistAsync();

            //gameDTO = mapper.Map<GameUpdateDTO>(game);

            //return CreatedAtAction("GetGame", new { id = game.Id }, gameDTO);
            return game.Id;
        }

        public async Task DeleteGame(int id)
        {
            var game = await UOW.GameRepository.GetAsync(id);
            if (game == null)
            {
                //Need to make a differece between not found and already deleted
                return;
            }

            UOW.GameRepository.Remove(game);
            await UOW.PersistAsync();
        }

        public async Task PatchGame(int id, JsonPatchDocument<GameUpdateDTO> patchDoc)
        {
            if (patchDoc == null) return;

            var gameToPatch = await UOW.GameRepository.GetAsync(id);

            if (gameToPatch== null) return;

            var dto = mapper.Map<GameUpdateDTO>(gameToPatch);

            //patchDoc.ApplyTo(dto, ModelState);

            TryValidateModel(dto);

            if (!ModelState.IsValid)
            {
                return;
            }

            mapper.Map(dto, gameToPatch);
            await UOW.PersistAsync();
        }

        //No calls to GameExists
        private async Task<bool> GameExists(int id)
        {
            return await UOW.GameRepository.AnyAsync(id);
        }
    }


}
