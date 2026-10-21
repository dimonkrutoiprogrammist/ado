using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Entity;
using System.Linq;

namespace GamesDb
{

    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Studio { get; set; } = "";
        public string Style { get; set; } = "";
        public DateTime ReleaseDate { get; set; }
    }

    public class GamesContext : DbContext
    {
        public DbSet<Game> Games { get; set; } = null;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=GamesDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    };

    class Program
    {
        static void Main(string[] args)
        {
            CreateDatabase();
            AddGames();
            GetGames();

            Console.ReadLine();
        }

        public string GameMode { get; set; } = "";
        public int CopiesSold { get; set; }

        static void CreateDatabase()
        {
            using (GamesContext db = new GamesContext())
            {
                db.Database.Migrate();
            }
        }

        static void AddGames()
        {
            using (GamesContext db = new GamesContext())
            {
                if (!db.Games.Any())
                {
                    db.Games.AddRange(
                        new Game
                        {
                            Name = "The Witcher 3",
                            Studio = "CD Projekt Red",
                            Style = "RPG",
                            ReleaseDate = new DateTime(2015, 5, 19)
                        },
                        new Game
                        {
                            Name = "Minecraft",
                            Studio = "Mojang",
                            Style = "Песочница",
                            ReleaseDate = new DateTime(2011, 11, 18)
                        },
                        new Game
                        {
                            Name = "Portal 2",
                            Studio = "Valve",
                            Style = "Головоломка",
                            ReleaseDate = new DateTime(2011, 4, 19)
                        });

                    db.SaveChanges();
                }
            }
        }

        static void GetGames()
        {
            using (GamesContext db = new GamesContext())
            {
                foreach (Game game in db.Games.ToList())
                {
                    Console.WriteLine(
                        game.Name + " | " +
                        game.Studio + " | " +
                        game.Style + " | " +
                        game.ReleaseDate.ToShortDateString());
                }
            }
        }
    }
}