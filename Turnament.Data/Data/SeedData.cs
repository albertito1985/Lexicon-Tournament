using Bogus;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;

namespace Tournament.Data.Data
{
    public static class SeedData
    {
        public static async Task Seed(TournamentContext db)
        {
            await db.Database.MigrateAsync();
            if (await db.TournamentDetails.AnyAsync())
            {
                return; // Database has been seeded
            }

            try
            {
                var tournaments = GenerateTournaments(4);
                db.AddRange(tournaments);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static List<TournamentDetails> GenerateTournaments(int nrOfTournaments)
        {
            string[] tournamentNames = [
              "Duel of the Fates",
              "HoloTable Clash",
              "The Force Showdown",
              "Galaxy's Edge Gauntlet",
              "Smuggler’s Standoff",
              "Jedi Trials Tournament",
              "Sith Lords’ Skirmish",
              "Battle for the Outer Rim",
              "Rise of the Force",
              "The Mandalorian Masters"
            ];

            var faker = new Faker<TournamentDetails>("sv").Rules((f, t) =>
            {
                t.Title = f.PickRandom(tournamentNames);
                t.StartDate = f.Date.Future().Date;
                t.Games = GenerateGames(f.Random.Int(min: 2, max: 10), t.StartDate);
            });
            return faker.Generate(nrOfTournaments);
        }

        private static List<Game> GenerateGames(int nrOfGames, DateTime startDate)
        {
            List<DateTime> dates = GenerateDates(startDate);
            int index = 0;
            string[] gameTitles = new string[]
            {
                "Duel of the Fates",
                "The Force Showdown",
                "Sith Lords’ Skirmish",
                "Jedi Trials",
                "Battle for the Outer Rim",
                "Echo Base Clash",
                "Rise of the Force",
                "Empire’s Gambit",
                "Rebellion Rumble",
                "The Kessel Run",
                "Coruscant Conflict",
                "Galaxy’s Edge Gauntlet",
                "Smuggler’s Standoff",
                "The HoloTable Heist",
                "Clash on Mustafar",
                "Tatooine Throwdown",
                "The Mandalorian Masters",
                "Lightsaber Legends",
                "Bounty Hunter Blitz",
                "Dark Side Dominion",
                "Hoth Frontline",
                "Knights of the New Republic",
                "The Phantom Duel",
                "Sabacc and Sabers",
                "Clone Wars Conquest",
                "Resistance Rising",
                "Shadow of the Empire",
                "Wookiee Warzone",
                "The Final Order",
                "Order 66 Open"
            };
            var faker = new Faker<Game>("sv").Rules((f, g) =>
            {
                g.Title = f.PickRandom(gameTitles);
                g.Time = dates[index++];
            });
            return faker.Generate(nrOfGames);
        }

        private static List<DateTime> GenerateDates(DateTime startDate)
        {
            return Enumerable.Range(0, 90)
                .Select(i => startDate.AddDays(i).Date.AddHours(8))
                .OrderBy(_ => Guid.NewGuid())
                .ToList();
        }
    }
}
