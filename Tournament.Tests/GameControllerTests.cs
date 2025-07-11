using AutoMapper;
using Azure;
using Domain.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Service.Contracts;
using System.Reflection.Metadata;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.Request;
using Tournament.Data.Data;
using Tournament.Presentation.Controllers;
using Tournament.Services;
using Xunit;
namespace Tournament.Tests
{
    public class GameControllerTests
    {
        private List<Game> games;
        private IEnumerable<GameDTO> gamesDTOs;
        private GameUpdateDTO gameUpdateDTO;
        private PagedList<Game> pagedGames;
        private IMapper? mapper;

        //Mock<IGameRepository> mockRepository = new();
        //Mock<IUnitOfWork> mockUOW = new();
        Mock<IServiceManager> mockServiceManager = new();
        Mock<IGameService> mockGameService = new();
        GamesController gamesController;

        public GameControllerTests()
        {
            //Configure Test Data
            games = new()
            {
                new Game
                {
                    Id = 1,
                    Title = "E Star Wars Unlimited: Galactic Showdown",
                    Time = new DateTime(2025, 6, 1, 8, 0, 0),
                    TournamentDetailsId = 101
                },
                new Game
                {
                    Id = 2,
                    Title = "D Star Wars Destiny: Core Worlds Clash",
                    Time = new DateTime(2025, 6, 5, 8, 0, 0),
                    TournamentDetailsId = 102
                },
                new Game
                {
                    Id = 3,
                    Title = "C Star Wars TCG: Jedi Masters League",
                    Time = new DateTime(2025, 6, 12, 8, 0, 0),
                    TournamentDetailsId = 103
                },
                new Game
                {
                    Id = 4,
                    Title = "B Star Wars Unlimited: Outer Rim Brawl",
                    Time = new DateTime(2025, 6, 18, 8, 0, 0),
                    TournamentDetailsId = 104
                },
                new Game
                {
                    Id = 5,
                    Title = "A Star Wars Destiny: Sith Trials",
                    Time = new DateTime(2025, 6, 25, 8, 0, 0),
                    TournamentDetailsId = 105
                }
            };
            gamesDTOs =new List<GameDTO>()
            {
                new GameDTO
                {
                    Title = "E Star Wars Unlimited: Galactic Showdown",
                    Time = new DateTime(2025, 6, 1, 8, 0, 0)
                },
                new GameDTO
                {
                    Title = "D Star Wars Destiny: Core Worlds Clash",
                    Time = new DateTime(2025, 6, 5, 8, 0, 0)
                },
                new GameDTO
                {
                    Title = "C Star Wars TCG: Jedi Masters League",
                    Time = new DateTime(2025, 6, 12, 8, 0, 0)
                },
                new GameDTO
                {
                    Title = "B Star Wars Unlimited: Outer Rim Brawl",
                    Time = new DateTime(2025, 6, 18, 8, 0, 0)
                },
                new GameDTO
                {
                    Title = "A Star Wars Destiny: Sith Trials",
                    Time = new DateTime(2025, 6, 25, 8, 0, 0)
                }
            };
            gameUpdateDTO = new()
            {
                Title = "Replacement title",
                Time = new DateTime(2025, 7, 1, 8, 0, 0),
                TournamentDetailsId = 3
            };
            pagedGames = new PagedList<Game>(games, 5, 1, 3);

            //Configure Mapper
            TournamentMappings myProfile = new();
            MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            mapper = new Mapper(configuration);

            //Configure Mocks
            mockServiceManager.Setup(sm => sm.GameService).Returns(mockGameService.Object);

            var httpContext = new DefaultHttpContext();
            gamesController = new(mockServiceManager.Object);
            gamesController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("Title", "2025-06-03T08:00:00", "2025-06-26T08:00:00")]
        public async void GetGame_ReturnsOkResult_WithExpectedParams_WhenFiltersAreApplied(string? title, string? startDate, string? endDate)
        {
            GameGetParamsDTO gameParams = new GameGetParamsDTO
            {
                OrderCriteria = title,
                StartTime = startDate!=null ? DateTime.Parse(startDate) : null,
                EndTime = endDate != null ? DateTime.Parse(endDate) : null
            };

            mockGameService.Setup(s => s.GetGame(It.IsAny<GameGetParamsDTO>(), false))
                .Returns(Task.FromResult((gamesDTOs, pagedGames.MetaData)));

            var result = await gamesController.GetGame(gameParams);

            mockGameService.Verify(o => o.GetGame(
                It.Is<GameGetParamsDTO>(g =>
                g.OrderCriteria == title &&
                g.StartTime == (startDate != null ? DateTime.Parse(startDate) : null) &&
                g.EndTime == (endDate != null ? DateTime.Parse(endDate) : null)
                    ),false), Times.Once);
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<GameDTO>>(okResult.Value);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void GetGameFromId_ReturnsOkResult_WhenGameExists_OrNotFound_WhenMissing(bool validId)
        {
            int mockId = 1;

            if (validId) mockGameService.Setup(s => s.GetGameFromId(It.IsAny<int>())).Returns(Task.FromResult(gamesDTOs.ElementAt(1)));
            else mockGameService.Setup(s => s.GetGameFromId(It.IsAny<int>())).ThrowsAsync(new GameNotFoundException(mockId));

            ActionResult<GameDTO> MethodActionresult;

            if (validId) MethodActionresult = await gamesController.GetGameFromId(mockId);
            else
            {
                await Assert.ThrowsAsync<GameNotFoundException>(() => gamesController.GetGameFromId(mockId));
                return;
            }

            var result = MethodActionresult.Result;

            mockGameService.Verify(o => o.GetGameFromId(
                It.Is<int>(x => x == mockId)), Times.Once);
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<GameDTO>(okResult.Value);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void GetGameFromTitle_ReturnsOkResult_WhenGameExists_OrNotFound_WhenMissing(bool validTitle)
        {
            string title = "E Star Wars Unlimited: Galactic Showdown";

            if(validTitle) mockGameService.Setup(s => s.GetGameFromTitle(It.IsAny<string>())).Returns(Task.FromResult(gamesDTOs.ElementAt(1)));
            else mockGameService.Setup(s => s.GetGameFromTitle(It.IsAny<string>())).ThrowsAsync(new GameTitleNotFoundException(title));

            ActionResult<GameDTO> MethodActionresult;

            if (validTitle) MethodActionresult = await gamesController.GetGameFromTitle(title);
            else
            {
                await Assert.ThrowsAsync<GameTitleNotFoundException>(() => gamesController.GetGameFromTitle(title));
                return;
            }

            var result = MethodActionresult.Result;

            mockGameService.Verify(s => s.GetGameFromTitle(
                It.Is<string>(x => x == title)), Times.Once);
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<GameDTO>(okResult.Value);

        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public async void PutGame_ReturnsExpectedResult_BasedOnGameExistence_AndDbConcurrencyError(bool gameExist, bool dBError)
        {
            int mockId = 1;

            if(!gameExist) mockGameService.Setup(s => s.PutGame(It.IsAny<int>(), It.IsAny<GameUpdateDTO>())).ThrowsAsync(new GameNotFoundException(mockId));
            else if (dBError) mockGameService.Setup(s => s.PutGame(It.IsAny<int>(), It.IsAny<GameUpdateDTO>())).ThrowsAsync(new DbUpdateConcurrencyException());

            if(!gameExist)
            {
                await Assert.ThrowsAsync<GameNotFoundException>(() => gamesController.PutGame(mockId, gameUpdateDTO));
                return;
            }
            else if (dBError)
            {
                await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => gamesController.PutGame(mockId, gameUpdateDTO));
                return;
            }

            var result = await gamesController.PutGame(mockId, gameUpdateDTO);

            mockGameService.Verify(s => s.PutGame(
                It.Is<int>(x => x == mockId),
                It.Is<GameUpdateDTO>(gud => 
                gud.Title == gameUpdateDTO.Title &&
                gud.Time == gameUpdateDTO.Time &&
                gud.TournamentDetailsId == gameUpdateDTO.TournamentDetailsId))
            , Times.Once);
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);

        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false,false)]
        public async void PostGame_ReturnsCreatedAt_WhenDTOIsValid_OrBadRequest_WhenInvalid(bool gameDTOValid, bool DBfail)
        {

            if(!gameDTOValid) mockGameService.Setup(s => s.PostGame(It.IsAny<GameUpdateDTO>())).ThrowsAsync(new GameBadRequestException("A tournament cannot have more than 10 games."));
            else if (DBfail) mockGameService.Setup(s => s.PostGame(It.IsAny<GameUpdateDTO>())).ThrowsAsync(new DbUpdateException());
            else mockGameService.Setup(s => s.PostGame(It.IsAny<GameUpdateDTO>())).ReturnsAsync(1);

            if (!gameDTOValid)
            {
                await Assert.ThrowsAsync<GameBadRequestException>(() => gamesController.PostGame(gameUpdateDTO));
                return;
            }
            
            if (DBfail)
            {
                await Assert.ThrowsAsync<DbUpdateException>(() => gamesController.PostGame(gameUpdateDTO));
                return;
            }
                var result = await gamesController.PostGame(gameUpdateDTO);

                mockGameService.Verify(s => s.PostGame(
                It.Is<GameUpdateDTO>(gud =>
                gud.Title == gameUpdateDTO.Title &&
                gud.Time == gameUpdateDTO.Time &&
                gud.TournamentDetailsId == gameUpdateDTO.TournamentDetailsId
                )), Times.Once);

                Assert.NotNull(result);

                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                Assert.Equal("GetGame", createdAtActionResult.ActionName);

                var returnedDto = Assert.IsType<GameUpdateDTO>(createdAtActionResult.Value);
                Assert.Equal(gameUpdateDTO.Title, returnedDto.Title);
                Assert.Equal(gameUpdateDTO.Time, returnedDto.Time);
                Assert.Equal(gameUpdateDTO.TournamentDetailsId, returnedDto.TournamentDetailsId);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async void DeleteGame_ReturnsExpectedResult_BasedOnGameExistence(bool validId, bool DBfail)
        {
            int mockId = 1;
            if (validId) mockGameService.Setup(s => s.DeleteGame(mockId)).ThrowsAsync(new GameNotFoundException(mockId));
            else if(DBfail) mockGameService.Setup(s => s.DeleteGame(mockId)).ThrowsAsync(new DbUpdateException());
            else mockGameService.Setup(s => s.DeleteGame(mockId)).Returns(Task.CompletedTask);

            if (validId)
            { 
                Assert.ThrowsAsync<GameNotFoundException>(() => gamesController.DeleteGame(mockId));
                return;
            }
            if (DBfail)
            { 
                Assert.ThrowsAsync<DbUpdateException>(() => gamesController.DeleteGame(mockId));
                return;
            }

            var result = await gamesController.DeleteGame(mockId);

            mockGameService.Verify(s => s.DeleteGame(
                It.Is<int>(x => x == mockId)), Times.Once);
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }


        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async void PatchGame_ReturnsCorrectResult_BasedOnPatchValidity_GameExistence_AndModelState(bool DBfail, bool gameIsValid)
        {
            int mockId = 1;

            if (!gameIsValid) mockGameService.Setup(s => s.PatchGame(It.IsAny<int>(), It.IsAny<JsonPatchDocument<GameUpdateDTO>>())).ThrowsAsync(new GameNotFoundException(1));
            else if (DBfail) mockGameService.Setup(s => s.PatchGame(It.IsAny<int>(), It.IsAny<JsonPatchDocument<GameUpdateDTO>>())).ThrowsAsync(new DbUpdateException());
            else mockGameService.Setup(s => s.PatchGame(It.IsAny<int>(), It.IsAny<JsonPatchDocument<GameUpdateDTO>>())).Returns(Task.CompletedTask);
            
            JsonPatchDocument<GameUpdateDTO> patchDoc = new JsonPatchDocument<GameUpdateDTO>();
            patchDoc.Replace(g => g.Title, "Patched Game");

            if(!gameIsValid)
            {
                await Assert.ThrowsAsync<GameNotFoundException>(() => gamesController.PatchGame(mockId, patchDoc));
                return;
            }

            if (DBfail)
            {
                await Assert.ThrowsAsync<DbUpdateException>(() => gamesController.PatchGame(mockId, patchDoc));
                return;
            }

            var result = await gamesController.PatchGame(mockId, patchDoc);

            Assert.NotNull(result);

            mockGameService.Verify(o => o.PatchGame(
                It.Is<int>(x => x == mockId), It.Is<JsonPatchDocument<GameUpdateDTO>>(x => x == patchDoc)), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}