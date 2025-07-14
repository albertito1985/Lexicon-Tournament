using AutoMapper;
using Domain.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
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
using Tournament.Data.Data;
using Tournament.Presentation.Controllers;
using Tournament.Services;
using Xunit;

namespace Tournament.Tests
{
    public class TournamentControllerTests
    {
        private List<TournamentDetails> tournaments;
        private IEnumerable<TournamentDetailsDTO> tournamentDTOs;
        private TournamentUpdateDTO tournamentUpdateDTO;
        private PagedList<TournamentDetailsDTO> pagedTournaments;
        private IMapper? mapper;

        Mock<IServiceManager> mockServiceManager = new();
        Mock<ITournamentService> mockTournamentService = new();
        TournamentDetailsController tournamentController;

        public TournamentControllerTests()
        {
            //Configure Test Data
            tournaments = new()
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
            tournamentDTOs = new List<TournamentDetailsDTO>
                {
                    new TournamentDetailsDTO
                    {
                        Title = "Jedi Trials Showdown",
                        StartDate = new DateTime(2025, 6, 10),
                        EndDate = new DateTime(2025, 6, 10, 14, 0, 0),
                        Games = new List<Game>
                        {
                            new Game { Id = 1, Title = "Yoda vs Dooku", Time = new DateTime(2025, 6, 10, 10, 0, 0), TournamentDetailsId = 1 },
                            new Game { Id = 2, Title = "Obi-Wan vs Maul", Time = new DateTime(2025, 6, 10, 12, 0, 0), TournamentDetailsId = 1 },
                            new Game { Id = 3, Title = "Mace Windu vs Grievous", Time = new DateTime(2025, 6, 10, 14, 0, 0), TournamentDetailsId = 1 }
                        }
                    },
                    new TournamentDetailsDTO
                    {
                        Title = "Galactic Civil War Finals",
                        StartDate = new DateTime(2025, 6, 12),
                        EndDate = new DateTime(2025, 6, 12, 13, 0, 0),
                        Games = new List<Game>
                        {
                            new Game { Id = 4, Title = "Luke vs Vader", Time = new DateTime(2025, 6, 12, 11, 0, 0), TournamentDetailsId = 2 },
                            new Game { Id = 5, Title = "Leia vs Palpatine", Time = new DateTime(2025, 6, 12, 13, 0, 0), TournamentDetailsId = 2 }
                        }
                    },
                    new TournamentDetailsDTO
                    {
                        Title = "Smugglers' Run Cup",
                        StartDate = new DateTime(2025, 6, 15),
                        EndDate = new DateTime(2025, 6, 15, 15, 0, 0),
                        Games = new List<Game>
                        {
                            new Game { Id = 6, Title = "Han vs Lando", Time = new DateTime(2025, 6, 15, 9, 30, 0), TournamentDetailsId = 3 },
                            new Game { Id = 7, Title = "Chewbacca vs Boba Fett", Time = new DateTime(2025, 6, 15, 11, 0, 0), TournamentDetailsId = 3 },
                            new Game { Id = 8, Title = "Lando vs Chewbacca", Time = new DateTime(2025, 6, 15, 13, 0, 0), TournamentDetailsId = 3 },
                            new Game { Id = 9, Title = "Han vs Boba Fett", Time = new DateTime(2025, 6, 15, 15, 0, 0), TournamentDetailsId = 3 }
                        }
                    },
                    new TournamentDetailsDTO
                    {
                        Title = "Clone Wars Open",
                        StartDate = new DateTime(2025, 6, 18),
                        EndDate = new DateTime(2025, 6, 18, 16, 0, 0),
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
            tournamentUpdateDTO = new() 
            {
                Title = "Replacement title",
                StartDate = new DateTime(2025, 7, 1, 8, 0, 0),
                Games = new List<Game>
                        {
                            new Game { Id = 1, Title = "Replacement Game 1", Time = new DateTime(2025, 6, 10, 10, 0, 0), TournamentDetailsId = 1 },
                            new Game { Id = 2, Title = "Replacement Game 2", Time = new DateTime(2025, 6, 10, 12, 0, 0), TournamentDetailsId = 1 },
                            new Game { Id = 3, Title = "Replacement Game 3", Time = new DateTime(2025, 6, 10, 14, 0, 0), TournamentDetailsId = 1 }
                        }
            };
            pagedTournaments = new PagedList<TournamentDetailsDTO>(tournamentDTOs, 5, 1, 3);

            //Configure Mapper
            TournamentMappings myProfile = new();
            MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            mapper = new Mapper(configuration);

            //Configure Mocks
            mockServiceManager.Setup(sm => sm.TournamentService).Returns(mockTournamentService.Object);

            var httpContext = new DefaultHttpContext();
            tournamentController = new(mockServiceManager.Object);
            tournamentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

        }

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

            mockTournamentService.Setup(ts => ts.GetTournamentDetails(It.IsAny<TournamentGetParamsDTO>(), false))
                .Returns(Task.FromResult((tournamentDTOs, pagedTournaments.MetaData)));

            var result = await tournamentController.GetTournamentDetails(tournamentParams);

            mockTournamentService.Verify(s => s.GetTournamentDetails(
                It.Is<TournamentGetParamsDTO>(TDDTO =>
                TDDTO.IncludeGames == includeGames &&
               TDDTO.OrderCriteria == orderCriteria &&
                TDDTO.StartDate == (startDate!=null ? DateTime.Parse(startDate) : null) &&
               TDDTO.EndDate == (endDate != null ? DateTime.Parse(endDate) : null)
                    ),It.Is<bool>(v => v == false)), Times.Once);
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<TournamentDetailsDTO>>(okResult.Value);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void GetTournamentFromId_ReturnsOkResult_WhenTournamentExists_OrNotFound_WhenMissing(bool validId)
        {
            int mockId = 1;
            if (validId) mockTournamentService.Setup(r => r.GetTournamentDetails(It.IsAny<int>())).ReturnsAsync(tournamentDTOs.ElementAt(1));
            else mockTournamentService.Setup(r => r.GetTournamentDetails(It.IsAny<int>())).Throws(new TournamentNotFoundException(mockId));


            if (!validId)
            {
                await Assert.ThrowsAsync<TournamentNotFoundException>(async () => 
                    await tournamentController.GetTournamentDetails(mockId));
                return;
            }

            var result = await tournamentController.GetTournamentDetails(mockId);

            mockTournamentService.Verify(s => s.GetTournamentDetails(
                It.Is<int>(x => x == mockId)), Times.Once);

            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<TournamentDetailsDTO>(okResult.Value);
        }

        [Fact]
        public async void PutTournament_ReturnsExpectedResult_BasedOnTournamentExistence_AndDbConcurrencyError()
        {
            int mockId = 1;
            mockTournamentService.Setup(r => r.PutTournamentDetails(It.IsAny<int>(), It.IsAny<TournamentUpdateDTO>())).Returns(Task.CompletedTask);

            TournamentUpdateDTO tournamentDTO = new()
            {
                Title = "Replaced Game Title",
                StartDate = new DateTime(2025, 6, 1, 10, 0, 0),
                Games = new List<Game>
                    {
                        new Game { Id = 1, Title = "Replaced game 1", Time = new DateTime(2025, 6, 10, 10, 0, 0), TournamentDetailsId = 1 },
                        new Game { Id = 2, Title = "Replaced game 2", Time = new DateTime(2025, 6, 10, 12, 0, 0), TournamentDetailsId = 1 },
                        new Game { Id = 3, Title = "Replaced game 3", Time = new DateTime(2025, 6, 10, 14, 0, 0), TournamentDetailsId = 1 }
                    }
            };

            var result = await tournamentController.PutTournamentDetails(mockId, tournamentDTO);

            mockTournamentService.Verify(s => s.PutTournamentDetails(
                It.Is<int>(x => x == mockId), It.Is<TournamentUpdateDTO>(DTO =>
                DTO.Title == tournamentDTO.Title &&
                DTO.StartDate == tournamentDTO.StartDate &&
                DTO.Games == tournamentDTO.Games)), Times.Once);
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void PostTournament_ReturnsCreatedAt_WhenDTOIsValid_OrBadRequest_WhenInvalid()
        {
            mockTournamentService.Setup(s => s.PostTournamentDetails(It.IsAny<TournamentDetailsDTO>()));

            TournamentDetailsDTO tournamentDetailsDTO = new ()
                {
                    Title = "New Game Title",
                    StartDate = new DateTime(2025, 6, 1, 10, 0, 0),
                    EndDate = new DateTime(2025, 6, 1, 10, 0, 0),
                    Games = new List<Game>
                    {
                        new Game { Id = 1, Title = "New game 1", Time = new DateTime(2025, 6, 10, 10, 0, 0), TournamentDetailsId = 1 },
                        new Game { Id = 2, Title = "New game 2", Time = new DateTime(2025, 6, 10, 12, 0, 0), TournamentDetailsId = 1 },
                        new Game { Id = 3, Title = "New game 3", Time = new DateTime(2025, 6, 10, 14, 0, 0), TournamentDetailsId = 1 }
                    }
            };

            var result = await tournamentController.PostTournamentDetails(tournamentDetailsDTO);

            mockTournamentService.Verify(s => s.PostTournamentDetails(
            It.Is<TournamentDetailsDTO>(DTO =>
            DTO.Title == tournamentDetailsDTO.Title &&
            DTO.StartDate == tournamentDetailsDTO.StartDate &&
            DTO.EndDate == tournamentDetailsDTO.EndDate &&
            DTO.Games == tournamentDetailsDTO.Games
            )), Times.Once);

            Assert.NotNull(result);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetTournamentDetails", createdAtActionResult.ActionName);

            var returnedDto = Assert.IsType<TournamentDetailsDTO>(createdAtActionResult.Value);
            Assert.Equal(tournamentDetailsDTO.Title, returnedDto.Title);
            Assert.Equal(tournamentDetailsDTO.StartDate, returnedDto.StartDate);
            Assert.Equal(tournamentDetailsDTO.EndDate, returnedDto.EndDate);
            Assert.Equal(tournamentDetailsDTO.Games, returnedDto.Games);
        }

        [Fact]
        public async void Deletetournament_ReturnsExpectedResult_BasedOntournamentExistence()
        {
            int mockId = 1;
            mockTournamentService.Setup(s => s.DeleteTournamentDetails(It.IsAny<int>()));

            var result = await tournamentController.DeleteTournamentDetails(mockId);

            mockTournamentService.Verify(s => s.DeleteTournamentDetails(
                It.Is<int>(x => x == mockId)), Times.Once);
            
            Assert.NotNull(result);

            mockTournamentService.Verify(s => s.DeleteTournamentDetails(
            It.Is<int>(x => x == mockId
            )), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }


        [Fact]
        public async void PatchGame_ReturnsCorrectResult_BasedOnPatchValidity_GameExistence_AndModelState()
        {
            int mockId = 1;
            JsonPatchDocument<TournamentUpdateDTO> patchDoc =  new JsonPatchDocument<TournamentUpdateDTO>();
                    patchDoc.Replace(g => g.Title, "Patched Game");

            mockTournamentService.Setup(s => s.PatchTournament(It.IsAny<int>(), It.IsAny<JsonPatchDocument<TournamentUpdateDTO>>())).Returns(Task.CompletedTask);

            var result = await tournamentController.PatchTournament(mockId, patchDoc);

            Assert.NotNull(result);

            mockTournamentService.Verify(s => s.PatchTournament(
                It.Is<int>(x => x == mockId), It.IsAny<JsonPatchDocument<TournamentUpdateDTO>>()), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
