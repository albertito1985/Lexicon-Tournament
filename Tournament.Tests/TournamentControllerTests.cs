using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.API.Controllers;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Xunit;

namespace Tournament.Tests
{
    public class TournamentControllerTests()
    {
        List<TournamentDetails> tournaments = new ()
        {
            new TournamentDetails
            {
                Id = 1,
                Title = "Jedi Trials Showdown",
                StartDate = new DateTime(2025, 6, 10),
                Games = new List<Game>
                {
                    new Game { Id = 1, Title = "Yoda vs Dooku", Time = new DateTime(2025, 6, 10, 10, 0, 0), TournamentDetailsId = 1 },
                    new Game { Id = 2, Title = "Obi-Wan vs Maul", Time = new DateTime(2025, 6, 10, 12, 0, 0), TournamentDetailsId = 1 },
                    new Game { Id = 3, Title = "Mace Windu vs Grievous", Time = new DateTime(2025, 6, 10, 14, 0, 0), TournamentDetailsId = 1 }
                }
            },
            new TournamentDetails
            {
                Id = 2,
                Title = "Galactic Civil War Finals",
                StartDate = new DateTime(2025, 6, 12),
                Games = new List<Game>
                {
                    new Game { Id = 4, Title = "Luke vs Vader", Time = new DateTime(2025, 6, 12, 11, 0, 0), TournamentDetailsId = 2 },
                    new Game { Id = 5, Title = "Leia vs Palpatine", Time = new DateTime(2025, 6, 12, 13, 0, 0), TournamentDetailsId = 2 }
                }
            },
            new TournamentDetails
            {
                Id = 3,
                Title = "Smugglers' Run Cup",
                StartDate = new DateTime(2025, 6, 15),
                Games = new List<Game>
                {
                    new Game { Id = 6, Title = "Han vs Lando", Time = new DateTime(2025, 6, 15, 9, 30, 0), TournamentDetailsId = 3 },
                    new Game { Id = 7, Title = "Chewbacca vs Boba Fett", Time = new DateTime(2025, 6, 15, 11, 0, 0), TournamentDetailsId = 3 },
                    new Game { Id = 8, Title = "Lando vs Chewbacca", Time = new DateTime(2025, 6, 15, 13, 0, 0), TournamentDetailsId = 3 },
                    new Game { Id = 9, Title = "Han vs Boba Fett", Time = new DateTime(2025, 6, 15, 15, 0, 0), TournamentDetailsId = 3 }
                }
            },
            new TournamentDetails
            {
                Id = 4,
                Title = "Clone Wars Open",
                StartDate = new DateTime(2025, 6, 18),
                Games = new List<Game>
                {
                    new Game { Id = 10, Title = "Anakin vs Ahsoka", Time = new DateTime(2025, 6, 18, 10, 0, 0), TournamentDetailsId = 4 },
                    new Game { Id = 11, Title = "Rex vs Cody", Time = new DateTime(2025, 6, 18, 11, 30, 0), TournamentDetailsId = 4 },
                    new Game { Id = 12, Title = "Ventress vs Kenobi", Time = new DateTime(2025, 6, 18, 13, 0, 0), TournamentDetailsId = 4 },
                    new Game { Id = 13, Title = "Anakin vs Ventress", Time = new DateTime(2025, 6, 18, 14, 30, 0), TournamentDetailsId = 4 },
                    new Game { Id = 14, Title = "Ahsoka vs Kenobi", Time = new DateTime(2025, 6, 18, 16, 0, 0), TournamentDetailsId = 4 }
                }
            }
        };

        static TournamentMappings myProfile = new();
        static MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper _mapper = new Mapper(configuration);

        [Theory]
        [InlineData(false, null, null, null)]
        [InlineData(false, "Title", "2025-06-09T08:00:00", "2025-06-20T08:00:00")]
        public async void GetTournamentDetails(bool includeGames, string? orderCriteria, string? startDate, string? endDate)
        {
            TournamentGetParamsDTO tournamentParams = new()
            {
                IncludeGames = includeGames,
                OrderCriteria = orderCriteria,
                StartDate = startDate!=null ? DateTime.Parse(startDate) : null,
                EndDate = endDate != null ? DateTime.Parse(endDate) : null
            };
            Mock<ITournamentRepository> repository = new Mock<ITournamentRepository>();
            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(uow => uow.TournamentRepository).Returns(repository.Object);
            repository.Setup(r => r.GetAllAsync(It.IsAny<TournamentGetParamsDTO>())).ReturnsAsync(tournaments);

            TournamentDetailsController tournamentsController = new TournamentDetailsController(mockUOW.Object, _mapper);

            var result = await tournamentsController.GetTournamentDetails(tournamentParams);

            mockUOW.Verify(o => o.TournamentRepository.GetAllAsync(
                It.Is<TournamentGetParamsDTO>(g =>
                g.IncludeGames == includeGames &&
               g.OrderCriteria == orderCriteria &&
                g.StartDate == (startDate!=null ? DateTime.Parse(startDate) : null) &&
               g.EndDate == (endDate != null ? DateTime.Parse(endDate) : null)
                    )), Times.Once);
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<TournamentDetailsDTO>>(okResult.Value);
        }

        //[Theory]
        //[InlineData(true)]
        //[InlineData(false)]
        //public async void GetGameFromId_ReturnsOkResult_WhenGameExists_OrNotFound_WhenMissing(bool validId)
        //{
        //    Mock<IGameRepository> repository = new Mock<IGameRepository>();
        //    Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
        //    mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);

        //    if (validId) repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync(games[1]);
        //    else repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync((Game?)null);

        //    GamesController gamesController = new GamesController(mockUOW.Object, _mapper);

        //    int mockId = 1;

        //    var result = await gamesController.GetGameFromId(mockId);

        //    mockUOW.Verify(o => o.GameRepository.GetAsync(
        //        It.Is<int>(x => x == mockId)), Times.Once);

        //    if (validId)
        //    {
        //        Assert.NotNull(result);
        //        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //        Assert.IsAssignableFrom<GameDTO>(okResult.Value);
        //    }
        //    else
        //    {
        //        Assert.IsType<NotFoundResult>(result.Result);
        //    }
        //}

        //[Theory]
        //[InlineData("E Star Wars Unlimited: Galactic Showdown")]
        //[InlineData("invalidTitle")]
        //public async void GetGameFromTitle_ReturnsOkResult_WhenGameExists_OrNotFound_WhenMissing(string title)
        //{
        //    Mock<IGameRepository> repository = new Mock<IGameRepository>();
        //    Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
        //    mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);


        //    repository.Setup(r => r.GetAsync(It.Is<string>(x => x != "invalidTitle"))).ReturnsAsync(games[1]);
        //    repository.Setup(r => r.GetAsync(It.Is<string>(x => x == "invalidTitle"))).ReturnsAsync((Game?)null);

        //    GamesController gamesController = new GamesController(mockUOW.Object, _mapper);

        //    var result = await gamesController.GetGameFromTitle(title);

        //    mockUOW.Verify(o => o.GameRepository.GetAsync(
        //        It.Is<string>(x => x == title)), Times.Once);

        //    if (title == "E Star Wars Unlimited: Galactic Showdown")
        //    {
        //        Assert.NotNull(result);
        //        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //        Assert.IsAssignableFrom<GameDTO>(okResult.Value);
        //    }
        //    else
        //    {
        //        Assert.IsType<NotFoundResult>(result.Result);
        //    }
        //}

        //[Theory]
        //[InlineData(1, false)]
        //[InlineData(1, true)]
        //[InlineData(6, false)]
        //public async void PutGame_ReturnsExpectedResult_BasedOnGameExistence_AndDbConcurrencyError(int id, bool DBError)
        //{
        //    Mock<IGameRepository> repository = new Mock<IGameRepository>();
        //    Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();

        //    mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);
        //    mockUOW.Setup(uow => uow.PersistAsync()).Returns(Task.CompletedTask);
        //    if (DBError)
        //    {
        //        mockUOW.Setup(uow => uow.PersistAsync()).ThrowsAsync(new DbUpdateConcurrencyException());
        //    }
        //    repository.Setup(r => r.GetAsync(It.Is<int>(x => x != 6))).ReturnsAsync(games[1]);
        //    repository.Setup(r => r.GetAsync(It.Is<int>(x => x == 6))).ReturnsAsync((Game?)null);

        //    repository.Setup(r => r.Update(It.IsAny<Game>()));

        //    GamesController gamesController = new GamesController(mockUOW.Object, _mapper);

        //    GameUpdateDTO gameDTO = new()
        //    {
        //        Title = "Updated Game Title",
        //        Time = new DateTime(2025, 6, 1, 10, 0, 0)
        //    };

        //    IActionResult? result;

        //    if (DBError)
        //    {
        //        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => gamesController.PutGame(id, gameDTO));
        //        return;
        //    }
        //    else
        //    {
        //        result = await gamesController.PutGame(id, gameDTO);
        //    }

        //    mockUOW.Verify(o => o.GameRepository.GetAsync(
        //        It.Is<int>(x => x == id)), Times.Once);
        //    Assert.NotNull(result);

        //    if (id == 1) // if the game exists on the first call
        //    {
        //        mockUOW.Verify(o => o.PersistAsync(), Times.Once);

        //        if (DBError) // if the update is successfull
        //        {
        //            var NoContentResult = Assert.IsType<NoContentResult>(result);
        //        }
        //    }
        //    else
        //    {
        //        Assert.IsType<NotFoundResult>(result);
        //    }
        //}

        //[Theory]
        //[InlineData(true)]
        //[InlineData(false)]
        //public async void PostGame_ReturnsCreatedAt_WhenDTOIsValid_OrBadRequest_WhenInvalid(bool gameDTOValid)
        //{
        //    Mock<IGameRepository> repository = new Mock<IGameRepository>();
        //    Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
        //    mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);
        //    mockUOW.Setup(uow => uow.PersistAsync()).Returns(Task.CompletedTask);

        //    repository.Setup(r => r.Add(It.IsAny<Game>()));

        //    GamesController gamesController = new GamesController(mockUOW.Object, _mapper);

        //    GameUpdateDTO? gameDTO = null;

        //    if (gameDTOValid)
        //    {
        //        gameDTO = new GameUpdateDTO
        //        {
        //            Title = "New Game Title",
        //            Time = new DateTime(2025, 6, 1, 10, 0, 0),
        //            TournamentDetailsId = 1
        //        };
        //    }

        //    var result = await gamesController.PostGame(gameDTO);

        //    if (gameDTOValid)
        //    {
        //        mockUOW.Verify(uow => uow.GameRepository.Add(
        //        It.Is<Game>(x =>
        //        x.Title == gameDTO.Title &&
        //        x.Time == gameDTO.Time &&
        //        x.TournamentDetailsId == gameDTO.TournamentDetailsId
        //        )), Times.Once);

        //        mockUOW.Verify(o => o.PersistAsync(), Times.Once);

        //        Assert.NotNull(result);

        //        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        //        Assert.Equal("GetGame", createdAtActionResult.ActionName);

        //        var returnedDto = Assert.IsType<GameUpdateDTO>(createdAtActionResult.Value);
        //        Assert.Equal(gameDTO.Title, returnedDto.Title);
        //        Assert.Equal(gameDTO.Time, returnedDto.Time);
        //        Assert.Equal(gameDTO.TournamentDetailsId, returnedDto.TournamentDetailsId);
        //    }
        //    else
        //    {
        //        Assert.IsType<BadRequestObjectResult>(result.Result);
        //    }
        //}

        //[Theory]
        //[InlineData(true)]
        //[InlineData(false)]
        //public async void DeleteGame_ReturnsExpectedResult_BasedOnGameExistence(bool validId)
        //{
        //    Mock<IGameRepository> repository = new Mock<IGameRepository>();
        //    Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
        //    mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);
        //    mockUOW.Setup(uow => uow.PersistAsync()).Returns(Task.CompletedTask);

        //    repository.Setup(r => r.Remove(It.IsAny<Game>()));
        //    int mockId = 1;
        //    if (validId) repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync(games[mockId]);
        //    else repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync((Game?)null);

        //    GamesController gamesController = new(mockUOW.Object, _mapper);

        //    var result = await gamesController.DeleteGame(mockId);

        //    mockUOW.Verify(o => o.GameRepository.GetAsync(
        //        It.Is<int>(x => x == mockId)), Times.Once);
        //    Assert.NotNull(result);

        //    if (validId)
        //    {
        //        mockUOW.Verify(uow => uow.GameRepository.Remove(
        //        It.Is<Game>(x =>
        //        x.Title == games[mockId].Title &&
        //        x.Time == games[mockId].Time &&
        //        x.TournamentDetailsId == games[mockId].TournamentDetailsId
        //        )), Times.Once);

        //        mockUOW.Verify(uow => uow.PersistAsync(), Times.Once);

        //        Assert.IsType<NoContentResult>(result);
        //    }
        //    else
        //    {
        //        Assert.IsType<NotFoundResult>(result);
        //    }

        //}


        //[Theory]
        //[InlineData(true, true, true)]
        //[InlineData(false, true, true)]
        //[InlineData(true, false, true)]
        //[InlineData(false, false, true)]
        //[InlineData(true, true, false)]
        //[InlineData(false, true, false)]
        //[InlineData(true, false, false)]
        //[InlineData(false, false, false)]
        //public async void PatchGame_ReturnsCorrectResult_BasedOnPatchValidity_GameExistence_AndModelState(bool patchIsValid, bool gameIsValid, bool validDTOModel)
        //{
        //    Mock<IGameRepository> repository = new Mock<IGameRepository>();
        //    Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
        //    mockUOW.Setup(uow => uow.GameRepository).Returns(repository.Object);
        //    mockUOW.Setup(uow => uow.PersistAsync()).Returns(Task.CompletedTask);

        //    //var patchDoc = new JsonPatchDocument<GameUpdateDTO>();
        //    JsonPatchDocument<GameUpdateDTO> patchDoc;

        //    switch (patchIsValid, validDTOModel)
        //    {

        //        case (true, false):
        //            patchDoc = new JsonPatchDocument<GameUpdateDTO>();
        //            patchDoc.Replace(g => g.Title, null);
        //            break;
        //        case (true, true):
        //            patchDoc = new JsonPatchDocument<GameUpdateDTO>();
        //            patchDoc.Replace(g => g.Title, "Patched Game");
        //            break;
        //        case (false, true):
        //        case (false, false):
        //            patchDoc = null;
        //            break;
        //        default:
        //            break;
        //    }

        //    int mockId = 1;
        //    if (gameIsValid) repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync(games[mockId]);
        //    else repository.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync((Game?)null);

        //    GamesController gamesController = new(mockUOW.Object, _mapper);
        //    gamesController.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext()
        //    };
        //    gamesController.ObjectValidator = new FakeObjectModelValidator();

        //    var result = await gamesController.PatchGame(mockId, patchDoc);

        //    Assert.NotNull(result);


        //    if (!patchIsValid)
        //    {
        //        Assert.IsType<BadRequestObjectResult>(result);
        //        return;
        //    }

        //    mockUOW.Verify(o => o.GameRepository.GetAsync(
        //        It.Is<int>(x => x == mockId)), Times.Once);

        //    if (!gameIsValid)
        //    {
        //        Assert.IsType<NotFoundObjectResult>(result);
        //        return;
        //    }

        //    if (!validDTOModel)
        //    {
        //        Assert.IsType<UnprocessableEntityObjectResult>(result);
        //        return;
        //    }

        //    mockUOW.Verify(uow => uow.PersistAsync(), Times.Once);
        //    Assert.IsType<NoContentResult>(result);
        //}
    }
}
