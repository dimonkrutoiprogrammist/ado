using System;
using System.Data.Entity;
using System.Linq;

namespace GamesDb
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Задание 1");
                Console.WriteLine("1. Поиск игры по названию");
                Console.WriteLine("2. Поиск игр по названию студии");
                Console.WriteLine("3. Поиск игры по названию студии и игры");
                Console.WriteLine("4. Поиск игр по стилю");
                Console.WriteLine("5. Поиск игр по году выпуска");

                Console.WriteLine();
                Console.WriteLine("Задание 2");
                Console.WriteLine("6. Однопользовательские игры");
                Console.WriteLine("7. Многопользовательские игры");
                Console.WriteLine("8. Игра с максимальными продажами");
                Console.WriteLine("9. Игра с минимальными продажами");
                Console.WriteLine("10. Топ-3 продаваемых игр");
                Console.WriteLine("11. Топ-3 наименее продаваемых игр");

                Console.WriteLine();
                Console.WriteLine("Задание 3");
                Console.WriteLine("12. Добавить игру");
                Console.WriteLine("13. Изменить игру");
                Console.WriteLine("14. Удалить игру");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SearchByGameName();
                        break;
                    case "2":
                        SearchByStudioName();
                        break;
                    case "3":
                        SearchByStudioAndGame();
                        break;
                    case "4":
                        SearchByStyle();
                        break;
                    case "5":
                        SearchByReleaseYear();
                        break;
                    case "6":
                        GetGamesByType("Однопользовательская");
                        break;
                    case "7":
                        GetGamesByType("Многопользовательская");
                        break;
                    case "8":
                        GetGameWithMaxSales();
                        break;
                    case "9":
                        GetGameWithMinSales();
                        break;
                    case "10":
                        GetTopThreeGames();
                        break;
                    case "11":
                        GetLeastSoldGames();
                        break;
                    case "12":
                        AddGame();
                        break;
                    case "13":
                        UpdateGame();
                        break;
                    case "14":
                        DeleteGame();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Такого пункта нет");
                        break;
                }
            }
        }

        static void SearchByGameName()
        {
            Console.Write("Введите название игры: ");
            string gameName = Console.ReadLine();

            using (GamesDbEntities db = new GamesDbEntities())
            {
                var games = db.Games
                    .Include(x => x.Studio)
                    .Where(x => x.GameName.Contains(gameName))
                    .ToList();

                foreach (var game in games)
                {
                    Console.WriteLine(
                        game.GameName + " | " + game.Studio.StudioName);
                }
            }
        }

        static void SearchByStudioName()
        {
            Console.Write("Введите название студии: ");
            string studioName = Console.ReadLine();

            using (GamesDbEntities db = new GamesDbEntities())
            {
                var games = db.Games
                    .Include(x => x.Studio)
                    .Where(x => x.Studio.StudioName.Contains(studioName))
                    .ToList();

                foreach (var game in games)
                {
                    Console.WriteLine(
                        game.GameName + " | " + game.Studio.StudioName);
                }
            }
        }

        static void SearchByStudioAndGame()
        {
            Console.Write("Введите название студии: ");
            string studioName = Console.ReadLine();

            Console.Write("Введите название игры: ");
            string gameName = Console.ReadLine();

            using (GamesDbEntities db = new GamesDbEntities())
            {
                var games = db.Games
                    .Include(x => x.Studio)
                    .Where(x => x.Studio.StudioName.Contains(studioName)
                             && x.GameName.Contains(gameName))
                    .ToList();

                foreach (var game in games)
                {
                    Console.WriteLine(
                        game.GameName + " | " + game.Studio.StudioName);
                }
            }
        }

        static void SearchByStyle()
        {
            Console.Write("Введите стиль игры: ");
            string style = Console.ReadLine();

            using (GamesDbEntities db = new GamesDbEntities())
            {
                var games = db.Games
                    .Include(x => x.Studio)
                    .Where(x => x.Style.Contains(style))
                    .ToList();

                foreach (var game in games)
                {
                    Console.WriteLine(
                        game.GameName + " | " + game.Style);
                }
            }
        }

        static void SearchByReleaseYear()
        {
            Console.Write("Введите год выпуска: ");
            int year = int.Parse(Console.ReadLine());

            using (GamesDbEntities db = new GamesDbEntities())
            {
                var games = db.Games
                    .Include(x => x.Studio)
                    .Where(x => x.ReleaseYear == year)
                    .ToList();

                foreach (var game in games)
                {
                    Console.WriteLine(
                        game.GameName + " | " + game.ReleaseYear);
                }
            }
        }

        static void GetGamesByType(string gameType)
        {
            using (GamesDbEntities db = new GamesDbEntities())
            {
                var games = db.Games
                    .Include(x => x.Studio)
                    .Where(x => x.GameType == gameType)
                    .ToList();

                foreach (var game in games)
                {
                    Console.WriteLine(
                        game.GameName + " | " + game.GameType);
                }
            }
        }

        static void GetGameWithMaxSales()
        {
            using (GamesDbEntities db = new GamesDbEntities())
            {
                var game = db.Games
                    .Include(x => x.Studio)
                    .OrderByDescending(x => x.CopiesSold)
                    .FirstOrDefault();

                if (game != null)
                {
                    Console.WriteLine(
                        game.GameName + " | Продано: " + game.CopiesSold);
                }
            }
        }

        static void GetGameWithMinSales()
        {
            using (GamesDbEntities db = new GamesDbEntities())
            {
                var game = db.Games
                    .Include(x => x.Studio)
                    .OrderBy(x => x.CopiesSold)
                    .FirstOrDefault();

                if (game != null)
                {
                    Console.WriteLine(
                        game.GameName + " | Продано: " + game.CopiesSold);
                }
            }
        }

        static void GetTopThreeGames()
        {
            using (GamesDbEntities db = new GamesDbEntities())
            {
                var games = db.Games
                    .Include(x => x.Studio)
                    .OrderByDescending(x => x.CopiesSold)
                    .Take(3)
                    .ToList();

                foreach (var game in games)
                {
                    Console.WriteLine(
                        game.GameName + " | Продано: " + game.CopiesSold);
                }
            }
        }

        static void GetLeastSoldGames()
        {
            using (GamesDbEntities db = new GamesDbEntities())
            {
                var games = db.Games
                    .Include(x => x.Studio)
                    .OrderBy(x => x.CopiesSold)
                    .Take(3)
                    .ToList();

                foreach (var game in games)
                {
                    Console.WriteLine(
                        game.GameName + " | Продано: " + game.CopiesSold);
                }
            }
        }


        static void AddGame()
        {
            Console.Write("Введите название игры: ");
            string gameName = Console.ReadLine();

            Console.Write("Введите название студии: ");
            string studioName = Console.ReadLine();

            using (GamesDbEntities db = new GamesDbEntities())
            {
                Studio studio = db.Studios
                    .FirstOrDefault(x => x.StudioName == studioName);

                if (studio == null)
                {
                    Console.WriteLine("Студия не найдена");
                    return;
                }

                Game existingGame = db.Games
                    .FirstOrDefault(x => x.GameName == gameName
                                      && x.StudioId == studio.StudioId);

                if (existingGame != null)
                {
                    Console.WriteLine("Такая игра уже есть у этой студии");
                    return;
                }

                Console.Write("Введите стиль игры: ");
                string style = Console.ReadLine();

                Console.Write("Введите год выпуска: ");
                int year = int.Parse(Console.ReadLine());

                Console.Write("Введите тип игры (1 — одиночная, 2 — многопользовательская): ");
                string gameTypeChoice = Console.ReadLine();

                string gameType = "Однопользовательская";

                if (gameTypeChoice == "2")
                    gameType = "Многопользовательская";

                Console.Write("Введите количество проданных копий: ");
                int copiesSold = int.Parse(Console.ReadLine());

                Game game = new Game
                {
                    GameName = gameName,
                    StudioId = studio.StudioId,
                    Style = style,
                    ReleaseYear = year,
                    GameType = gameType,
                    CopiesSold = copiesSold
                };

                db.Games.Add(game);
                db.SaveChanges();

                Console.WriteLine("Игра добавлена");
            }
        }

        static void UpdateGame()
        {
            using (GamesDbEntities db = new GamesDbEntities())
            {
                var games = db.Games
                    .Include(x => x.Studio)
                    .ToList();

                foreach (var item in games)
                {
                    Console.WriteLine(
                        item.GameId + ". " + item.GameName +
                        " | " + item.Studio.StudioName);
                }

                Console.Write("Введите ID игры: ");
                int id = int.Parse(Console.ReadLine());

                Game game = db.Games.Find(id);

                if (game == null)
                {
                    Console.WriteLine("Игра не найдена");
                    return;
                }

                Console.WriteLine("1. Название игры");
                Console.WriteLine("2. Студия");
                Console.WriteLine("3. Стиль");
                Console.WriteLine("4. Год выпуска");
                Console.WriteLine("5. Тип игры");
                Console.WriteLine("6. Количество проданных копий");
                Console.Write("Что изменить: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Введите новое название: ");
                    game.GameName = Console.ReadLine();
                }
                else if (choice == "2")
                {
                    Console.Write("Введите название новой студии: ");
                    string studioName = Console.ReadLine();

                    Studio studio = db.Studios
                        .FirstOrDefault(x => x.StudioName == studioName);

                    if (studio == null)
                    {
                        Console.WriteLine("Студия не найдена");
                        return;
                    }

                    game.StudioId = studio.StudioId;
                }
                else if (choice == "3")
                {
                    Console.Write("Введите новый стиль: ");
                    game.Style = Console.ReadLine();
                }
                else if (choice == "4")
                {
                    Console.Write("Введите новый год выпуска: ");
                    game.ReleaseYear = int.Parse(Console.ReadLine());
                }
                else if (choice == "5")
                {
                    Console.Write("Введите тип игры (1 - одиночная, 2 — многопользовательская): ");
                    string gameTypeChoice = Console.ReadLine();

                    if (gameTypeChoice == "1")
                        game.GameType = "Однопользовательская";
                    else
                        game.GameType = "Многопользовательская";
                }
                else if (choice == "6")
                {
                    Console.Write("Введите новое количество проданных копий: ");
                    game.CopiesSold = int.Parse(Console.ReadLine());
                }

                db.SaveChanges();
                Console.WriteLine("Данные изменены");
            }
        }

        static void DeleteGame()
        {
            Console.Write("Введите название игры: ");
            string gameName = Console.ReadLine();

            Console.Write("Введите название студии: ");
            string studioName = Console.ReadLine();

            using (GamesDbEntities db = new GamesDbEntities())
            {
                Game game = db.Games
                    .Include(x => x.Studio)
                    .FirstOrDefault(x => x.GameName == gameName
                                      && x.Studio.StudioName == studioName);

                if (game == null)
                {
                    Console.WriteLine("Игра не найдена");
                    return;
                }

                Console.WriteLine(
                    game.GameName + " | " + game.Studio.StudioName);

                Console.Write("Удалить игру? (да/нет): ");
                string answer = Console.ReadLine();

                if (answer.ToLower() == "да")
                {
                    db.Games.Remove(game);
                    db.SaveChanges();
                    Console.WriteLine("Игра удалена");
                }
            }
        }
    }
}