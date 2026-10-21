using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Linq;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Capital { get; set; }
    public long Population { get; set; }
    public double Area { get; set; }
    public string Continent { get; set; }
}

public class City
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    public string Name { get; set; }
    public long Population { get; set; }
    public bool IsCapital { get; set; }
}

class Program
{
    static string connectionString =
        @"Data Source=(localdb)\MSSQLSERVER;Initial Catalog=Countries;Integrated Security=True;";

    static SqlConnection connection = null;

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\n1 - Подключение / отключение");
            Console.WriteLine("2 - Отчёты");
            Console.WriteLine("3 - Дополнительные отчёты");
            Console.WriteLine("4 - Дополнительные отчёты");
            Console.WriteLine("0 - Выход");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Task1();
                    break;
                case "2":
                    Task2();
                    break;
                case "3":
                    Task3();
                    break;
                case "4":
                    Task4();
                    break;
                case "0":
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                    return;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
        }
    }

    static void Task1()
    {
        Console.WriteLine("\nЗадание 1");
        Console.WriteLine("1 - Подключиться");
        Console.WriteLine("2 - Отключиться");
        Console.Write("Выберите действие: ");

        string choice = Console.ReadLine();

        if (choice == "1")
        {
            try
            {
                if (connection != null &&
                    connection.State == ConnectionState.Open)
                {
                    Console.WriteLine("Соединение уже открыто");
                    return;
                }

                connection = new SqlConnection(connectionString);
                connection.Open();

                Console.WriteLine("Подключение успешно");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка подключения: " + ex.Message);

                if (connection != null)
                {
                    connection.Dispose();
                    connection = null;
                }
            }
        }
        else if (choice == "2")
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
                connection = null;

                Console.WriteLine("Соединение закрыто");
            }
            else
            {
                Console.WriteLine("Соединение уже закрыто");
            }
        }
        else
        {
            Console.WriteLine("Неверный выбор");
        }
    }

    static void Task2()
    {
        if (connection == null ||
            connection.State != ConnectionState.Open)
        {
            Console.WriteLine("Сначала подключитесь к базе данных");
            return;
        }

        Console.WriteLine("\nЗадание 2");
        Console.WriteLine("1 - Вся информация о странах");
        Console.WriteLine("2 - Частичная информация о странах");
        Console.WriteLine("3 - Информация о конкретной стране");
        Console.WriteLine("4 - Города конкретной страны");
        Console.Write("Выберите отчёт: ");

        string choice = Console.ReadLine();
        SqlCommand command = null;

        if (choice == "1")
        {
            command = new SqlCommand(
                @"SELECT c.Name AS Country,
                         cap.Name AS Capital,
                         c.Population AS CountryPopulation,
                         cap.Population AS CapitalPopulation,
                         c.Area,
                         c.Continent
                  FROM Countries c
                  LEFT JOIN Cities cap
                    ON cap.CountryId = c.Id AND cap.IsCapital = 1",
                connection);
        }
        else if (choice == "2")
        {
            Console.Write("Количество полей (от 1 до 4): ");
            int count;

            if (!int.TryParse(Console.ReadLine(), out count) ||
                count < 1 || count > 4)
            {
                Console.WriteLine("Неверное количество полей");
                return;
            }

            string[] fields =
            {
                "Name",
                "Population",
                "Area",
                "Continent"
            };

            string columns = string.Join(", ", fields.Take(count));

            command = new SqlCommand(
                "SELECT " + columns + " FROM Countries",
                connection);
        }
        else if (choice == "3")
        {
            Console.Write("Название страны: ");
            string name = Console.ReadLine();

            command = new SqlCommand(
                @"SELECT c.Name AS Country,
                         cap.Name AS Capital,
                         c.Population AS CountryPopulation,
                         cap.Population AS CapitalPopulation,
                         c.Area,
                         c.Continent
                  FROM Countries c
                  LEFT JOIN Cities cap
                    ON cap.CountryId = c.Id AND cap.IsCapital = 1
                  WHERE c.Name = @Name",
                connection);

            command.Parameters.AddWithValue("@Name", name);
        }
        else if (choice == "4")
        {
            Console.Write("Название страны: ");
            string name = Console.ReadLine();

            command = new SqlCommand(
                @"SELECT city.Name AS City,
                         city.Population,
                         city.IsCapital
                  FROM Cities city
                  INNER JOIN Countries c ON city.CountryId = c.Id
                  WHERE c.Name = @Name",
                connection);

            command.Parameters.AddWithValue("@Name", name);
        }
        else
        {
            Console.WriteLine("Неверный выбор");
            return;
        }

        DataTable table = new DataTable();

        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            adapter.Fill(table);

        StringBuilder report = new StringBuilder();

        foreach (DataColumn column in table.Columns)
            report.Append(column.ColumnName).Append("\t");

        report.AppendLine();

        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn column in table.Columns)
                report.Append(row[column] == DBNull.Value ? "" : row[column].ToString())
                      .Append("\t");

            report.AppendLine();
        }

        if (table.Rows.Count == 0)
            report.AppendLine("Данные не найдены");

        Console.WriteLine("\n1 - Вывести на экран");
        Console.WriteLine("2 - Сохранить в файл");
        Console.Write("Выберите способ вывода: ");

        if (Console.ReadLine() == "2")
        {
            Console.Write("Имя файла: ");
            string fileName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "Report.txt";

            File.WriteAllText(fileName, report.ToString(), Encoding.UTF8);
            Console.WriteLine("Отчёт сохранён: " + fileName);
        }
        else
        {
            Console.WriteLine(report.ToString());
        }
    }

    static void Task3()
    {
        if (connection == null ||
            connection.State != ConnectionState.Open)
        {
            Console.WriteLine("Сначала подключитесь к базе данных");
            return;
        }

        Console.WriteLine("\nЗадание 3");
        Console.WriteLine("1 - Страны на заданную букву");
        Console.WriteLine("2 - Столицы на заданную букву");
        Console.WriteLine("3 - Топ-3 столицы с наименьшим населением");
        Console.WriteLine("4 - Топ-3 страны с наименьшим населением");
        Console.WriteLine("5 - Среднее население столиц по частям света");
        Console.Write("Выберите отчёт: ");

        string choice = Console.ReadLine();
        SqlCommand command = null;

        if (choice == "1")
        {
            Console.Write("Введите букву: ");
            string letter = Console.ReadLine();

            command = new SqlCommand(
                "SELECT Name FROM Countries WHERE Name LIKE @Letter + '%'",
                connection);

            command.Parameters.AddWithValue("@Letter", letter);
        }
        else if (choice == "2")
        {
            Console.Write("Введите букву: ");
            string letter = Console.ReadLine();

            command = new SqlCommand(
                @"SELECT city.Name AS Capital,
                         city.Population,
                         c.Name AS Country
                  FROM Cities city
                  INNER JOIN Countries c ON city.CountryId = c.Id
                  WHERE city.IsCapital = 1
                    AND city.Name LIKE @Letter + '%'",
                connection);

            command.Parameters.AddWithValue("@Letter", letter);
        }
        else if (choice == "3")
        {
            command = new SqlCommand(
                @"SELECT TOP 3 city.Name AS Capital,
                         city.Population,
                         c.Name AS Country
                  FROM Cities city
                  INNER JOIN Countries c ON city.CountryId = c.Id
                  WHERE city.IsCapital = 1
                  ORDER BY city.Population ASC",
                connection);
        }
        else if (choice == "4")
        {
            command = new SqlCommand(
                @"SELECT TOP 3 Name, Population
                  FROM Countries
                  ORDER BY Population ASC",
                connection);
        }
        else if (choice == "5")
        {
            command = new SqlCommand(
                @"SELECT c.Continent,
                         AVG(CAST(city.Population AS FLOAT)) AS AveragePopulation
                  FROM Countries c
                  INNER JOIN Cities city ON city.CountryId = c.Id
                  WHERE city.IsCapital = 1
                  GROUP BY c.Continent",
                connection);
        }
        else
        {
            Console.WriteLine("Неверный выбор");
            return;
        }

        DataTable table = new DataTable();

        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            adapter.Fill(table);

        StringBuilder report = new StringBuilder();

        foreach (DataColumn column in table.Columns)
            report.Append(column.ColumnName).Append("\t");

        report.AppendLine();

        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn column in table.Columns)
                report.Append(row[column] == DBNull.Value ? "" : row[column].ToString())
                      .Append("\t");

            report.AppendLine();
        }

        if (table.Rows.Count == 0)
            report.AppendLine("Данные не найдены");

        Console.WriteLine("\n1 - Вывести на экран");
        Console.WriteLine("2 - Сохранить в файл");
        Console.Write("Выберите способ вывода: ");

        if (Console.ReadLine() == "2")
        {
            Console.Write("Имя файла: ");
            string fileName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "Report.txt";

            File.WriteAllText(fileName, report.ToString(), Encoding.UTF8);
            Console.WriteLine("Отчёт сохранён: " + fileName);
        }
        else
        {
            Console.WriteLine(report.ToString());
        }
    }

    static void Task4()
    {
        if (connection == null ||
            connection.State != ConnectionState.Open)
        {
            Console.WriteLine("Сначала подключитесь к базе данных");
            return;
        }

        Console.WriteLine("\nЗадание 4");
        Console.WriteLine("1 - Топ-3 стран с наименьшим населением по частям света");
        Console.WriteLine("2 - Топ-3 стран с наибольшим населением по частям света");
        Console.WriteLine("3 - Среднее население городов конкретной страны");
        Console.WriteLine("4 - Город с наименьшим населением в конкретной стране");
        Console.Write("Выберите отчёт: ");

        string choice = Console.ReadLine();
        SqlCommand command = null;

        if (choice == "1")
        {
            command = new SqlCommand(
                @"WITH CountryRanking AS
                  (
                      SELECT Name, Continent, Population,
                             ROW_NUMBER() OVER
                             (PARTITION BY Continent ORDER BY Population ASC) AS Number
                      FROM Countries
                  )
                  SELECT Name, Continent, Population
                  FROM CountryRanking
                  WHERE Number <= 3
                  ORDER BY Continent, Population ASC",
                connection);
        }
        else if (choice == "2")
        {
            command = new SqlCommand(
                @"WITH CountryRanking AS
                  (
                      SELECT Name, Continent, Population,
                             ROW_NUMBER() OVER
                             (PARTITION BY Continent ORDER BY Population DESC) AS Number
                      FROM Countries
                  )
                  SELECT Name, Continent, Population
                  FROM CountryRanking
                  WHERE Number <= 3
                  ORDER BY Continent, Population DESC",
                connection);
        }
        else if (choice == "3")
        {
            Console.Write("Название страны: ");
            string name = Console.ReadLine();

            command = new SqlCommand(
                @"SELECT c.Name AS Country,
                         AVG(CAST(city.Population AS FLOAT)) AS AverageCityPopulation
                  FROM Countries c
                  INNER JOIN Cities city ON city.CountryId = c.Id
                  WHERE c.Name = @Name
                  GROUP BY c.Name",
                connection);

            command.Parameters.AddWithValue("@Name", name);
        }
        else if (choice == "4")
        {
            Console.Write("Название страны: ");
            string name = Console.ReadLine();

            command = new SqlCommand(
                @"SELECT TOP 1 city.Name AS City, city.Population
                  FROM Cities city
                  INNER JOIN Countries c ON city.CountryId = c.Id
                  WHERE c.Name = @Name
                  ORDER BY city.Population ASC",
                connection);

            command.Parameters.AddWithValue("@Name", name);
        }
        else
        {
            Console.WriteLine("Неверный выбор");
            return;
        }

        DataTable table = new DataTable();

        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            adapter.Fill(table);

        StringBuilder report = new StringBuilder();

        foreach (DataColumn column in table.Columns)
            report.Append(column.ColumnName).Append("\t");

        report.AppendLine();

        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn column in table.Columns)
                report.Append(row[column] == DBNull.Value ? "" : row[column].ToString())
                      .Append("\t");

            report.AppendLine();
        }

        if (table.Rows.Count == 0)
            report.AppendLine("Данные не найдены");

        Console.WriteLine("\n1 - Вывести на экран");
        Console.WriteLine("2 - Сохранить в файл");
        Console.Write("Выберите способ вывода: ");

        if (Console.ReadLine() == "2")
        {
            Console.Write("Имя файла: ");
            string fileName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "Report.txt";

            File.WriteAllText(fileName, report.ToString(), Encoding.UTF8);
            Console.WriteLine("Отчёт сохранён: " + fileName);
        }
        else
        {
            Console.WriteLine(report.ToString());
        }
    }
}