using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Reflection.Metadata;
using Tournament.API.Controllers;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Xunit;
namespace Tournament.Tests
{
    public class GameControllerTests()
    {
        List<Game> games = new()
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

        static TournamentMappings myProfile = new();
        static MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper _mapper = new Mapper(configuration);

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
            Mock<IGameRepository> repository = new Mock<IGameRepository>();
            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);
            repository.Setup(r => r.GetAllAsync(It.IsAny<GameGetParamsDTO>())).ReturnsAsync(games);

            GamesController gamesController = new GamesController(mockUOW.Object, _mapper);

            var result = await gamesController.GetGame(gameParams);

            mockUOW.Verify(o => o.GameRepository.GetAllAsync(
                It.Is<GameGetParamsDTO>(g =>
                g.OrderCriteria == title &&
                g.StartTime == (startDate != null ? DateTime.Parse(startDate) : null) &&
                g.EndTime == (endDate != null ? DateTime.Parse(endDate) : null)
                    )), Times.Once);
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<GameDTO>>(okResult.Value);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void GetGameFromId_ReturnsOkResult_WhenGameExists_OrNotFound_WhenMissing(bool validId)
        {
            Mock<IGameRepository> repository = new Mock<IGameRepository>();
            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);

            if(validId) repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync(games[1]);
            else repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync((Game?)null);

            GamesController gamesController = new GamesController(mockUOW.Object, _mapper);
            
            int mockId = 1;

            var result = await gamesController.GetGameFromId(mockId);

            mockUOW.Verify(o => o.GameRepository.GetAsync(
                It.Is<int>(x => x == mockId)), Times.Once);

            if(validId)
            {
                Assert.NotNull(result);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                Assert.IsAssignableFrom<GameDTO>(okResult.Value);
            }
            else
            {
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Theory]
        [InlineData("E Star Wars Unlimited: Galactic Showdown")]
        [InlineData("invalidTitle")]
        public async void GetGameFromTitle_ReturnsOkResult_WhenGameExists_OrNotFound_WhenMissing(string title)
        {
            Mock<IGameRepository> repository = new Mock<IGameRepository>();
            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);


            repository.Setup(r => r.GetAsync(It.Is<string>(x => x != "invalidTitle"))).ReturnsAsync(games[1]);
            repository.Setup(r => r.GetAsync(It.Is<string>(x => x == "invalidTitle"))).ReturnsAsync((Game?)null);

            GamesController gamesController = new GamesController(mockUOW.Object, _mapper);

            var result = await gamesController.GetGameFromTitle(title);

            mockUOW.Verify(o => o.GameRepository.GetAsync(
                It.Is<string>(x => x == title)), Times.Once);

            if (title == "E Star Wars Unlimited: Galactic Showdown")
            {
                Assert.NotNull(result);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                Assert.IsAssignableFrom<GameDTO>(okResult.Value);
            }
            else
            {
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(1, true)]
        [InlineData(6, false)]
        public async void PutGame_ReturnsExpectedResult_BasedOnGameExistence_AndDbConcurrencyError(int id, bool DBError)
        {
            Mock<IGameRepository> repository = new Mock<IGameRepository>();
            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();

            mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);
            mockUOW.Setup(uow => uow.PersistAsync()).Returns(Task.CompletedTask);
            if (DBError)
            {
                mockUOW.Setup(uow => uow.PersistAsync()).ThrowsAsync(new DbUpdateConcurrencyException());
            }
            repository.Setup(r => r.GetAsync(It.Is<int>(x => x != 6))).ReturnsAsync(games[1]);
            repository.Setup(r => r.GetAsync(It.Is<int>(x => x == 6))).ReturnsAsync((Game?)null);

            repository.Setup(r => r.Update(It.IsAny<Game>()));

            GamesController gamesController = new GamesController(mockUOW.Object, _mapper);

            GameUpdateDTO gameDTO = new()
            {
                Title = "Updated Game Title",
                Time = new DateTime(2025, 6, 1, 10, 0, 0)
            };

            IActionResult? result;

            if (DBError)
            {
                await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => gamesController.PutGame(id, gameDTO));
                return;
            }
            else
            {
                result = await gamesController.PutGame(id, gameDTO);
            }

            mockUOW.Verify(o => o.GameRepository.GetAsync(
                It.Is<int>(x => x == id)), Times.Once);
            Assert.NotNull(result);

            if (id == 1) // if the game exists on the first call
            {
                mockUOW.Verify(o => o.PersistAsync(), Times.Once);

                if (DBError) // if the update is successfull
                {
                    var NoContentResult = Assert.IsType<NoContentResult>(result);
                }
            }
            else
            {
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void PostGame_ReturnsCreatedAt_WhenDTOIsValid_OrBadRequest_WhenInvalid(bool gameDTOValid)
        {
            Mock<IGameRepository> repository = new Mock<IGameRepository>();
            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);
            mockUOW.Setup(uow => uow.PersistAsync()).Returns(Task.CompletedTask);

            repository.Setup(r => r.Add(It.IsAny<Game>()));
          
            GamesController gamesController = new GamesController(mockUOW.Object, _mapper);

            GameUpdateDTO? gameDTO = null;

            if(gameDTOValid)
            {
                gameDTO = new GameUpdateDTO
                {
                    Title = "New Game Title",
                    Time = new DateTime(2025, 6, 1, 10, 0, 0),
                    TournamentDetailsId = 1
                };
            }

            var result = await gamesController.PostGame(gameDTO);

            if (gameDTOValid)
            {
                mockUOW.Verify(uow => uow.GameRepository.Add(
                It.Is<Game>(x =>
                x.Title == gameDTO.Title &&
                x.Time == gameDTO.Time &&
                x.TournamentDetailsId == gameDTO.TournamentDetailsId
                )), Times.Once);

                mockUOW.Verify(o => o.PersistAsync(), Times.Once);

                Assert.NotNull(result);

                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                Assert.Equal("GetGame", createdAtActionResult.ActionName);

                var returnedDto = Assert.IsType<GameUpdateDTO>(createdAtActionResult.Value);
                Assert.Equal(gameDTO.Title, returnedDto.Title);
                Assert.Equal(gameDTO.Time, returnedDto.Time);
                Assert.Equal(gameDTO.TournamentDetailsId, returnedDto.TournamentDetailsId);
            }
            else
            {
                Assert.IsType<BadRequestObjectResult>(result.Result);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void DeleteGame_ReturnsExpectedResult_BasedOnGameExistence(bool validId)
        {
            Mock<IGameRepository> repository = new Mock<IGameRepository>();
            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);
            mockUOW.Setup(uow => uow.PersistAsync()).Returns(Task.CompletedTask);

            repository.Setup(r => r.Remove(It.IsAny<Game>()));
            int mockId = 1;
            if (validId) repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync(games[mockId]);
            else repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync((Game?)null);

            GamesController gamesController = new(mockUOW.Object, _mapper);
            
            var result = await gamesController.DeleteGame(mockId);

            mockUOW.Verify(o => o.GameRepository.GetAsync(
                It.Is<int>(x => x == mockId)), Times.Once);
            Assert.NotNull(result);

            if (validId)
            {
                mockUOW.Verify(uow => uow.GameRepository.Remove(
                It.Is<Game>(x =>
                x.Title == games[mockId].Title &&
                x.Time == games[mockId].Time &&
                x.TournamentDetailsId == games[mockId].TournamentDetailsId
                )), Times.Once);

                mockUOW.Verify(uow => uow.PersistAsync(), Times.Once);

                Assert.IsType<NoContentResult>(result);
            }
            else
            {
                Assert.IsType<NotFoundResult>(result);
            }

        }


        [Theory]
        [InlineData(true, true, true)]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, false, true)]
        [InlineData(true, true, false)]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, false, false)]
        public async void PatchGame(bool patchIsValid, bool gameIsValid, bool validDTOModel)
        {
            Mock<IGameRepository> repository = new Mock<IGameRepository>();
            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);
            mockUOW.Setup(uow => uow.PersistAsync()).Returns(Task.CompletedTask);

            //var patchDoc = new JsonPatchDocument<GameUpdateDTO>();
            JsonPatchDocument<GameUpdateDTO> patchDoc;

            switch(patchIsValid,validDTOModel)
            {

                case (true,false):
                    patchDoc = new JsonPatchDocument<GameUpdateDTO>();
                    patchDoc.Replace(g => g.Title, null);
                    break;
                case (true,true):
                    patchDoc = new JsonPatchDocument<GameUpdateDTO>();
                    patchDoc.Replace(g => g.Title, "Patched Game");
                    break;
                case (false,true):
                case (false,false):
                    patchDoc = null;
                    break;
                default:
                    break;
            }

            int mockId = 1;
            if (gameIsValid) repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync(games[mockId]);
            else repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync((Game?)null);

            GamesController gamesController = new(mockUOW.Object, _mapper);
            gamesController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            gamesController.ObjectValidator = new FakeObjectModelValidator();

            var result = await gamesController.PatchGame(mockId, patchDoc);

            Assert.NotNull(result);
            

            if (!patchIsValid)
            {
                Assert.IsType<BadRequestObjectResult>(result);
                return;
            }

            mockUOW.Verify(o => o.GameRepository.GetAsync(
                It.Is<int>(x => x == mockId)), Times.Once);

            if (!gameIsValid)
            {
                Assert.IsType<NotFoundObjectResult>(result);
                return;
            }

            if (!validDTOModel)
            {
                Assert.IsType<UnprocessableEntityObjectResult>(result);
                return;
            }

            mockUOW.Verify(uow => uow.PersistAsync(), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}