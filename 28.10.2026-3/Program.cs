using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

public class Team
{
    public int TeamID { get; set; }
    public string TeamName { get; set; }
    public string City { get; set; }
}

public class Player
{
    public int PlayerID { get; set; }
    public string FullName { get; set; }
    public string Country { get; set; }
    public int PlayerNumber { get; set; }
    public string Position { get; set; }
    public int TeamID { get; set; }
}

public class Match
{
    public int MatchID { get; set; }
    public int HomeTeamID { get; set; }
    public int AwayTeamID { get; set; }
    public int HomeTeamGoals { get; set; }
    public int AwayTeamGoals { get; set; }
    public DateTime MatchDate { get; set; }
}

public class MatchGoal
{
    public int GoalID { get; set; }
    public int MatchID { get; set; }
    public int PlayerID { get; set; }
}

public class LaLigaContext : DbContext
{
    public LaLigaContext() : base(@"Server=(localdb)\mssqllocaldb;Database=LaLigaDB;Trusted_Connection=True;") { }

    public DbSet<Team> Teams { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<MatchGoal> MatchGoals { get; set; }
}

class Program
{
    static void Main()
    {
        using (LaLigaContext db = new LaLigaContext())
        {
            DateTime testDate = new DateTime(2026, 10, 27);
            string testTeam = "Реал Мадрид";

            Console.WriteLine("=== Задание 1 ===");
            ShowGoalDifference(db);

            Console.WriteLine("\n=== Задание 2 ===");
            ShowAllMatches(db);

            Console.WriteLine("\n=== Задание 3 ===");
            ShowMatchesByDate(db, testDate);

            Console.WriteLine("\n=== Задание 4 ===");
            ShowTeamMatches(db, testTeam);

            Console.WriteLine("\n=== Задание 5 ===");
            ShowGoalsByDate(db, testDate);

            Console.WriteLine("\n--- Тестирование операций ---");
            AddMatch(db, "Реал Мадрид", "Барселона", 2, 1, new DateTime(2026, 11, 15));
            AddMatch(db, "Реал Мадрид", "Барселона", 0, 0, new DateTime(2026, 11, 15));
            EditMatch(db, "Реал Мадрид", "Барселона", new DateTime(2026, 11, 15), "Реал Мадрид", "Барселона", 3, 3, new DateTime(2026, 11, 15));
            DeleteMatch(db, "Реал Мадрид", "Барселона", new DateTime(2026, 11, 15));
        }
    }

    static Team FindTeam(LaLigaContext db, string teamName)
    {
        return db.Teams.FirstOrDefault(t => t.TeamName == teamName);
    }

    static string GetTeamName(LaLigaContext db, int teamID)
    {
        Team team = db.Teams.FirstOrDefault(t => t.TeamID == teamID);
        return team == null ? "Неизвестно" : team.TeamName;
    }

    static void ShowGoalDifference(LaLigaContext db)
    {
        List<Team> teams = db.Teams.ToList();
        List<Match> matches = db.Matches.ToList();

        foreach (Team team in teams)
        {
            int scored = 0, conceded = 0;
            foreach (Match match in matches)
            {
                if (match.HomeTeamID == team.TeamID)
                {
                    scored += match.HomeTeamGoals;
                    conceded += match.AwayTeamGoals;
                }
                if (match.AwayTeamID == team.TeamID)
                {
                    scored += match.AwayTeamGoals;
                    conceded += match.HomeTeamGoals;
                }
            }
            Console.WriteLine($"{team.TeamName}: разница голов = {scored - conceded}");
        }
    }

    static void ShowAllMatches(LaLigaContext db)
    {
        List<Match> matches = db.Matches.ToList();
        if (matches.Count == 0)
        {
            Console.WriteLine("Матчей пока нет");
            return;
        }
        foreach (Match match in matches) ShowOneMatch(db, match);
    }

    static void ShowOneMatch(LaLigaContext db, Match match)
    {
        string homeName = GetTeamName(db, match.HomeTeamID);
        string awayName = GetTeamName(db, match.AwayTeamID);

        Console.WriteLine($"[{match.MatchDate.ToShortDateString()}] {homeName} {match.HomeTeamGoals}:{match.AwayTeamGoals} {awayName}");

        List<MatchGoal> goals = db.MatchGoals.Where(g => g.MatchID == match.MatchID).ToList();
        foreach (MatchGoal goal in goals)
        {
            var player = db.Players.FirstOrDefault(p => p.PlayerID == goal.PlayerID);
            if (player != null) Console.WriteLine($"  Автор гола: {player.FullName}");
        }
    }

    static void ShowMatchesByDate(LaLigaContext db, DateTime date)
    {
        List<Match> matches = db.Matches.ToList();
        bool found = false;

        foreach (Match match in matches)
        {
            if (match.MatchDate.Date == date.Date)
            {
                ShowOneMatch(db, match);
                found = true;
            }
        }
        if (!found) Console.WriteLine("На эту дату матчей нет");
    }

    static void ShowTeamMatches(LaLigaContext db, string teamName)
    {
        Team team = FindTeam(db, teamName);
        if (team == null)
        {
            Console.WriteLine("Команда не найдена");
            return;
        }

        List<Match> matches = db.Matches.ToList();
        bool found = false;

        foreach (Match match in matches)
        {
            if (match.HomeTeamID == team.TeamID || match.AwayTeamID == team.TeamID)
            {
                ShowOneMatch(db, match);
                found = true;
            }
        }
        if (!found) Console.WriteLine("У этой команды пока нет матчей");
    }

    static void ShowGoalsByDate(LaLigaContext db, DateTime date)
    {
        List<Match> matches = db.Matches.ToList();
        List<MatchGoal> goals = db.MatchGoals.ToList();
        bool found = false;

        foreach (Match match in matches)
        {
            if (match.MatchDate.Date == date.Date)
            {
                foreach (MatchGoal goal in goals)
                {
                    if (goal.MatchID == match.MatchID)
                    {
                        var player = db.Players.FirstOrDefault(p => p.PlayerID == goal.PlayerID);
                        if (player != null)
                        {
                            Console.WriteLine($"Игрок: {player.FullName}");
                            found = true;
                        }
                    }
                }
            }
        }
        if (!found) Console.WriteLine("За эту дату забитых голов не найдено");
    }

    static void AddMatch(LaLigaContext db, string homeName, string awayName, int homeGoals, int awayGoals, DateTime date)
    {
        Team homeTeam = FindTeam(db, homeName);
        Team awayTeam = FindTeam(db, awayName);

        if (homeTeam == null || awayTeam == null)
        {
            Console.WriteLine("Одна из команд не найдена в базе данных");
            return;
        }

        List<Match> matches = db.Matches.ToList();
        foreach (Match match in matches)
        {
            bool sameTeams = match.HomeTeamID == homeTeam.TeamID && match.AwayTeamID == awayTeam.TeamID;
            if (sameTeams && match.MatchDate.Date == date.Date)
            {
                Console.WriteLine("Такой матч уже есть в базе данных");
                return;
            }
        }

        Match newMatch = new Match
        {
            HomeTeamID = homeTeam.TeamID,
            AwayTeamID = awayTeam.TeamID,
            HomeTeamGoals = homeGoals,
            AwayTeamGoals = awayGoals,
            MatchDate = date
        };

        db.Matches.Add(newMatch);
        db.SaveChanges();
        Console.WriteLine("Матч добавлен.");
    }

    static void EditMatch(LaLigaContext db, string oldHomeName, string oldAwayName, DateTime oldDate, string newHomeName, string newAwayName, int newHomeGoals, int newAwayGoals, DateTime newDate)
    {
        Team oldHomeTeam = FindTeam(db, oldHomeName);
        Team oldAwayTeam = FindTeam(db, oldAwayName);
        Team newHomeTeam = FindTeam(db, newHomeName);
        Team newAwayTeam = FindTeam(db, newAwayName);

        if (oldHomeTeam == null || oldAwayTeam == null || newHomeTeam == null || newAwayTeam == null)
        {
            Console.WriteLine("Не удалось найти одну из команд");
            return;
        }

        List<Match> matches = db.Matches.ToList();
        Match matchToEdit = null;

        foreach (Match match in matches)
        {
            if (match.HomeTeamID == oldHomeTeam.TeamID && match.AwayTeamID == oldAwayTeam.TeamID && match.MatchDate.Date == oldDate.Date)
            {
                matchToEdit = match;
                break;
            }
        }

        if (matchToEdit == null)
        {
            Console.WriteLine("Матч для изменения не найден");
            return;
        }

        foreach (Match otherMatch in matches)
        {
            if (otherMatch.MatchID != matchToEdit.MatchID &&
                otherMatch.HomeTeamID == newHomeTeam.TeamID &&
                otherMatch.AwayTeamID == newAwayTeam.TeamID &&
                otherMatch.MatchDate.Date == newDate.Date)
            {
                Console.WriteLine("Матч с такими командами и датой уже существует");
                return;
            }
        }

        matchToEdit.HomeTeamID = newHomeTeam.TeamID;
        matchToEdit.AwayTeamID = newAwayTeam.TeamID;
        matchToEdit.HomeTeamGoals = newHomeGoals;
        matchToEdit.AwayTeamGoals = newAwayGoals;
        matchToEdit.MatchDate = newDate;

        db.SaveChanges();
        Console.WriteLine("Данные матча изменены");
    }

    static void DeleteMatch(LaLigaContext db, string homeName, string awayName, DateTime date)
    {
        Team homeTeam = FindTeam(db, homeName);
        Team awayTeam = FindTeam(db, awayName);

        if (homeTeam == null || awayTeam == null)
        {
            Console.WriteLine("Не удалось найти одну из команд");
            return;
        }

        List<Match> matches = db.Matches.ToList();
        Match matchToDelete = null;

        foreach (Match match in matches)
        {
            if (match.HomeTeamID == homeTeam.TeamID && match.AwayTeamID == awayTeam.TeamID && match.MatchDate.Date == date.Date)
            {
                matchToDelete = match;
                break;
            }
        }

        if (matchToDelete == null)
        {
            Console.WriteLine("Матч не найден");
            return;
        }

        Console.Write("Удалить этот матч? (y/n): ");
        string answer = Console.ReadLine();

        if (answer?.ToLower() == "y")
        {
            List<MatchGoal> goals = db.MatchGoals.Where(g => g.MatchID == matchToDelete.MatchID).ToList();
            foreach (MatchGoal goal in goals) db.MatchGoals.Remove(goal);

            db.Matches.Remove(matchToDelete);
            db.SaveChanges();
            Console.WriteLine("Матч удалён");
        }
        else
        {
            Console.WriteLine("Удаление отменено");
        }
    }

    static void AddGoal(LaLigaContext db, string homeName, string awayName, DateTime date, string playerName)
    {
        Team homeTeam = FindTeam(db, homeName);
        Team awayTeam = FindTeam(db, awayName);
        Player player = db.Players.FirstOrDefault(p => p.FullName == playerName);

        if (homeTeam == null || awayTeam == null)
        {
            Console.WriteLine("Не удалось найти команды.");
            return;
        }

        if (player == null)
        {
            Console.WriteLine("Игрок не найден.");
            return;
        }

        List<Match> matches = db.Matches.ToList();
        Match foundMatch = null;

        foreach (Match match in matches)
        {
            if (match.HomeTeamID == homeTeam.TeamID && match.AwayTeamID == awayTeam.TeamID && match.MatchDate.Date == date.Date)
            {
                foundMatch = match;
                break;
            }
        }

        if (foundMatch == null)
        {
            Console.WriteLine("Матч не найден");
            return;
        }

        MatchGoal newGoal = new MatchGoal
        {
            MatchID = foundMatch.MatchID,
            PlayerID = player.PlayerID
        };

        db.MatchGoals.Add(newGoal);
        db.SaveChanges();
        Console.WriteLine("Информация о голе добавлена");
    }
}