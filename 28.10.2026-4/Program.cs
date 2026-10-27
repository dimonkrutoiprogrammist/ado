using System;
using System.Collections.Generic;
using System.Linq;

class Team
{
    public int Id;
    public string Name;

    public Team(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

class Player
{
    public int Id;
    public string Name;
    public int TeamId;

    public Player(int id, string name, int teamId)
    {
        Id = id;
        Name = name;
        TeamId = teamId;
    }
}

class Match
{
    public int Id;
    public int HomeTeamId;
    public int AwayTeamId;
    public int HomeGoals;
    public int AwayGoals;
    public DateTime Date;
}

class Goal
{
    public int Id;
    public int MatchId;
    public int PlayerId;

    public Goal(int id, int matchId, int playerId)
    {
        Id = id;
        MatchId = matchId;
        PlayerId = playerId;
    }
}

class ScorerInfo
{
    public string Name;
    public string TeamName;
    public int Goals;

    public ScorerInfo(string name, string teamName, int goals)
    {
        Name = name;
        TeamName = teamName;
        Goals = goals;
    }
}

class TeamInfo
{
    public string Name;
    public int GoalsFor;
    public int GoalsAgainst;
    public int Points;
}

class Program
{
    static List<Team> teams = new List<Team>();
    static List<Player> players = new List<Player>();
    static List<Match> matches = new List<Match>();
    static List<Goal> goals = new List<Goal>();
    static Random random = new Random();
    static int nextMatchId = 1;
    static int nextGoalId = 1;

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        LoadInitialData();

        while (true)
        {
            Console.WriteLine("\n---------------- МЕНЮ ----------------");
            Console.WriteLine(" 1. Добавить случайные матчи");
            Console.WriteLine(" 2. Топ-3 бомбардиров команды");
            Console.WriteLine(" 3. Лучший бомбардир команды");
            Console.WriteLine(" 4. Топ-3 бомбардиров чемпионата");
            Console.WriteLine(" 5. Лучший бомбардир чемпионата");
            Console.WriteLine(" 6. Топ-3 команд по забитым голам");
            Console.WriteLine(" 7. Команда с наибольшим числом забитых голов");
            Console.WriteLine(" 8. Топ-3 команд по пропущенным голам");
            Console.WriteLine(" 9. Команда с наименьшим числом пропущенных голов");
            Console.WriteLine("10. Топ-3 команд по очкам");
            Console.WriteLine("11. Команда с наибольшим количеством очков");
            Console.WriteLine("12. Топ-3 команд по меньшему числу очков");
            Console.WriteLine("13. Команда с наименьшим количеством очков");
            Console.WriteLine("14. Разница забитых и пропущенных голов");
            Console.WriteLine("15. Показать все матчи");
            Console.WriteLine("16. Показать матчи за дату");
            Console.WriteLine("17. Показать матчи команды");
            Console.WriteLine("18. Показать авторов голов за дату");
            Console.WriteLine("19. Добавить матч");
            Console.WriteLine("20. Изменить матч");
            Console.WriteLine("21. Удалить матч");
            Console.WriteLine(" 0. Выход");
            Console.Write("Ваш выбор: ");

            string answer = Console.ReadLine();
            if (answer == "0") break;

            switch (answer)
            {
                case "1": AddRandomMatches(); break;
                case "2": ShowTeamScorers(3); break;
                case "3": ShowTeamScorers(1); break;
                case "4": ShowScorers(0, 3); break;
                case "5": ShowScorers(0, 1); break;
                case "6": ShowTeamStatistics(1, 3); break;
                case "7": ShowTeamStatistics(1, 1); break;
                case "8": ShowTeamStatistics(2, 3); break;
                case "9": ShowTeamStatistics(2, 1); break;
                case "10": ShowTeamStatistics(3, 3); break;
                case "11": ShowTeamStatistics(3, 1); break;
                case "12": ShowTeamStatistics(4, 3); break;
                case "13": ShowTeamStatistics(4, 1); break;
                case "14": ShowTeamStatistics(5, 0); break;
                case "15": ShowMatches(0, "", DateTime.MinValue); break;
                case "16": ShowMatches(1, "", ReadDate("Введите дату (ГГГГ-ММ-ДД): ")); break;
                case "17": ShowMatches(2, ReadText("Название команды: "), DateTime.MinValue); break;
                case "18": ShowGoalsByDate(ReadDate("Введите дату (ГГГГ-ММ-ДД): ")); break;
                case "19": AddOneMatch(); break;
                case "20": EditOneMatch(); break;
                case "21": DeleteOneMatch(); break;
                default: Console.WriteLine("Неверный пункт меню"); break;
            }
        }
    }

    static void LoadInitialData()
    {
        teams.Add(new Team(1, "Манчестер Сити"));
        teams.Add(new Team(2, "Ливерпуль"));
        teams.Add(new Team(3, "Арсенал"));
        teams.Add(new Team(4, "Челси"));

        players.Add(new Player(1, "Холанд", 1));
        players.Add(new Player(2, "Де Брюйне", 1));
        players.Add(new Player(3, "Фоден", 1));
        players.Add(new Player(4, "Салах", 2));
        players.Add(new Player(5, "Нуньес", 2));
        players.Add(new Player(6, "Диас", 2));
        players.Add(new Player(7, "Сака", 3));
        players.Add(new Player(8, "Эдегор", 3));
        players.Add(new Player(9, "Жезус", 3));
        players.Add(new Player(10, "Палмер", 4));
        players.Add(new Player(11, "Джексон", 4));
        players.Add(new Player(12, "Мадуэке", 4));

        AddMatchData(1, 2, 3, 2, new DateTime(2026, 9, 12));
        AddMatchData(3, 4, 1, 1, new DateTime(2026, 9, 15));
        AddMatchData(2, 3, 2, 0, new DateTime(2026, 9, 20));
        AddMatchData(4, 1, 1, 3, new DateTime(2026, 9, 25));
    }

    static void AddRandomMatches()
    {
        int count = ReadInt("Сколько матчей добавить? ");
        for (int i = 0; i < count; i++)
        {
            int home = random.Next(1, teams.Count + 1);
            int away = random.Next(1, teams.Count + 1);
            while (away == home) away = random.Next(1, teams.Count + 1);

            int homeGoals = random.Next(0, 5);
            int awayGoals = random.Next(0, 5);
            DateTime date = new DateTime(2026, 1, 1).AddDays(random.Next(0, 300));

            AddMatchData(home, away, homeGoals, awayGoals, date);
        }
        Console.WriteLine($"Добавлено матчей: {count}");
    }

    static void ShowTeamScorers(int count)
    {
        string teamName = ReadText("Введите название команды: ");
        Team team = teams.FirstOrDefault(t => t.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase));
        if (team == null) { Console.WriteLine("Команда не найдена"); return; }
        ShowScorers(team.Id, count);
    }

    static void ShowScorers(int teamId, int count)
    {
        var list = new List<ScorerInfo>();
        foreach (Player player in players)
        {
            if (teamId != 0 && player.TeamId != teamId) continue;

            int goalsCount = goals.Count(g => g.PlayerId == player.Id);
            if (goalsCount > 0)
            {
                Team team = teams.First(t => t.Id == player.TeamId);
                list.Add(new ScorerInfo(player.Name, team.Name, goalsCount));
            }
        }

        list = list.OrderByDescending(s => s.Goals).ToList();
        if (list.Count == 0) { Console.WriteLine("Забитых голов пока нет"); return; }

        int max = (count == 0 || count > list.Count) ? list.Count : count;
        for (int i = 0; i < max; i++)
            Console.WriteLine($"{i + 1}. {list[i].Name} ({list[i].TeamName}) — голов: {list[i].Goals}");
    }

    static void ShowTeamStatistics(int type, int count)
    {
        var list = new List<TeamInfo>();
        foreach (Team team in teams)
        {
            var info = new TeamInfo { Name = team.Name, GoalsFor = 0, GoalsAgainst = 0, Points = 0 };
            foreach (Match match in matches)
            {
                if (match.HomeTeamId == team.Id)
                {
                    info.GoalsFor += match.HomeGoals;
                    info.GoalsAgainst += match.AwayGoals;
                    if (match.HomeGoals > match.AwayGoals) info.Points += 3;
                    else if (match.HomeGoals == match.AwayGoals) info.Points += 1;
                }
                if (match.AwayTeamId == team.Id)
                {
                    info.GoalsFor += match.AwayGoals;
                    info.GoalsAgainst += match.HomeGoals;
                    if (match.AwayGoals > match.HomeGoals) info.Points += 3;
                    else if (match.AwayGoals == match.HomeGoals) info.Points += 1;
                }
            }
            list.Add(info);
        }

        switch (type)
        {
            case 1: list = list.OrderByDescending(x => x.GoalsFor).ToList(); break;
            case 2: list = list.OrderBy(x => x.GoalsAgainst).ToList(); break;
            case 3: list = list.OrderByDescending(x => x.Points).ToList(); break;
            case 4: list = list.OrderBy(x => x.Points).ToList(); break;
        }

        int max = (count == 0 || count > list.Count) ? list.Count : count;
        for (int i = 0; i < max; i++)
        {
            if (type == 1) Console.WriteLine($"{i + 1}. {list[i].Name} — забито: {list[i].GoalsFor}");
            else if (type == 2) Console.WriteLine($"{i + 1}. {list[i].Name} — пропущено: {list[i].GoalsAgainst}");
            else if (type == 3 || type == 4) Console.WriteLine($"{i + 1}. {list[i].Name} — очки: {list[i].Points}");
            else Console.WriteLine($"{list[i].Name} — разница: {list[i].GoalsFor - list[i].GoalsAgainst}");
        }
    }

    static void ShowMatches(int type, string teamName, DateTime date)
    {
        bool found = false;
        foreach (Match match in matches)
        {
            Team home = teams.First(t => t.Id == match.HomeTeamId);
            Team away = teams.First(t => t.Id == match.AwayTeamId);

            if (type == 1 && match.Date.Date != date.Date) continue;
            if (type == 2 && !home.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase) && !away.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase)) continue;

            Console.WriteLine($"{match.Id}. {match.Date:dd.MM.yyyy} — {home.Name} {match.HomeGoals}:{match.AwayGoals} {away.Name}");
            found = true;
        }
        if (!found) Console.WriteLine("Матчи не найдены");
    }

    static void ShowGoalsByDate(DateTime date)
    {
        bool found = false;
        foreach (Goal goal in goals)
        {
            Match match = matches.FirstOrDefault(m => m.Id == goal.MatchId);
            if (match != null && match.Date.Date == date.Date)
            {
                Player player = players.First(p => p.Id == goal.PlayerId);
                Console.WriteLine($"Автор гола: {player.Name}");
                found = true;
            }
        }
        if (!found) Console.WriteLine("За эту дату голов нет");
    }

    static void AddOneMatch()
    {
        string homeName = ReadText("Команда-хозяин: ");
        string awayName = ReadText("Команда-гость: ");
        int homeGoals = ReadInt("Голы хозяев: ");
        int awayGoals = ReadInt("Голы гостей: ");
        DateTime date = ReadDate("Дата матча (ГГГГ-ММ-ДД): ");

        Team home = teams.FirstOrDefault(t => t.Name.Equals(homeName, StringComparison.OrdinalIgnoreCase));
        Team away = teams.FirstOrDefault(t => t.Name.Equals(awayName, StringComparison.OrdinalIgnoreCase));

        if (home == null || away == null) { Console.WriteLine("Команда не найдена"); return; }
        if (home.Id == away.Id) { Console.WriteLine("Команда не может играть сама с собой"); return; }

        if (matches.Any(m => m.HomeTeamId == home.Id && m.AwayTeamId == away.Id && m.Date.Date == date.Date))
        {
            Console.WriteLine("Такой матч уже существует"); return;
        }

        AddMatchData(home.Id, away.Id, homeGoals, awayGoals, date);
        Console.WriteLine("Матч успешно добавлен");
    }

    static void EditOneMatch()
    {
        int id = ReadInt("Введите ID матча для изменения: ");
        Match match = matches.FirstOrDefault(m => m.Id == id);
        if (match == null) { Console.WriteLine("Матч не найден"); return; }

        string homeName = ReadText("Новая команда-хозяин: ");
        string awayName = ReadText("Новая команда-гость: ");
        int homeGoals = ReadInt("Новый счёт хозяев: ");
        int awayGoals = ReadInt("Новый счёт гостей: ");
        DateTime date = ReadDate("Новая дата: ");

        Team home = teams.FirstOrDefault(t => t.Name.Equals(homeName, StringComparison.OrdinalIgnoreCase));
        Team away = teams.FirstOrDefault(t => t.Name.Equals(awayName, StringComparison.OrdinalIgnoreCase));
        if (home == null || away == null || home.Id == away.Id) { Console.WriteLine("Ошибка в названиях команд"); return; }

        match.HomeTeamId = home.Id;
        match.AwayTeamId = away.Id;
        match.HomeGoals = homeGoals;
        match.AwayGoals = awayGoals;
        match.Date = date;

        goals.RemoveAll(g => g.MatchId == match.Id);
        AddRandomGoalScorers(match);
        Console.WriteLine("Матч обновлён");
    }

    static void DeleteOneMatch()
    {
        string homeName = ReadText("Команда-хозяин: ");
        string awayName = ReadText("Команда-гость: ");
        DateTime date = ReadDate("Дата матча: ");

        Team home = teams.FirstOrDefault(t => t.Name.Equals(homeName, StringComparison.OrdinalIgnoreCase));
        Team away = teams.FirstOrDefault(t => t.Name.Equals(awayName, StringComparison.OrdinalIgnoreCase));
        if (home == null || away == null) { Console.WriteLine("Команда не найдена"); return; }

        Match match = matches.FirstOrDefault(m => m.HomeTeamId == home.Id && m.AwayTeamId == away.Id && m.Date.Date == date.Date);
        if (match == null) { Console.WriteLine("Матч не найден"); return; }

        Console.Write("Удалить матч? (y/n): ");
        if (Console.ReadLine().Trim().ToLower() == "y")
        {
            goals.RemoveAll(g => g.MatchId == match.Id);
            matches.Remove(match);
            Console.WriteLine("Матч удалён");
        }
        else Console.WriteLine("Отмена удаления");
    }

    static void AddMatchData(int homeId, int awayId, int homeGoals, int awayGoals, DateTime date)
    {
        var match = new Match
        {
            Id = nextMatchId++,
            HomeTeamId = homeId,
            AwayTeamId = awayId,
            HomeGoals = homeGoals,
            AwayGoals = awayGoals,
            Date = date.Date
        };
        matches.Add(match);
        AddRandomGoalScorers(match);
    }

    static void AddRandomGoalScorers(Match match)
    {
        for (int i = 0; i < match.HomeGoals; i++)
        {
            int playerId = GetRandomPlayerId(match.HomeTeamId);
            goals.Add(new Goal(nextGoalId++, match.Id, playerId));
        }
        for (int i = 0; i < match.AwayGoals; i++)
        {
            int playerId = GetRandomPlayerId(match.AwayTeamId);
            goals.Add(new Goal(nextGoalId++, match.Id, playerId));
        }
    }

    static int GetRandomPlayerId(int teamId)
    {
        var teamPlayers = players.Where(p => p.TeamId == teamId).ToList();
        return teamPlayers[random.Next(teamPlayers.Count)].Id;
    }

    static int ReadInt(string message)
    {
        Console.Write(message);
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int value) && value >= 0) return value;
            Console.Write("Введите целое неотрицательное число: ");
        }
    }

    static DateTime ReadDate(string message)
    {
        Console.Write(message);
        while (true)
        {
            if (DateTime.TryParse(Console.ReadLine(), out DateTime date)) return date.Date;
            Console.Write("Неверный формат. Введите дату ещё раз: ");
        }
    }

    static string ReadText(string message)
    {
        Console.Write(message);
        return Console.ReadLine().Trim();
    }
}