using System;
using System.Data;
using System.Data.SqlClient;

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

    static void Main(string[] args)
    {
        Console.WriteLine("1 - Добавление ");
        Console.WriteLine("2 - Обновление");
        Console.WriteLine("3 - Удаление");
        Console.WriteLine("4 - Хранимые);
        Console.Write("Выберите -  ");

        int choice = Convert.ToInt32(Console.ReadLine());

        switch (choice)
        {
            case 1:
                Task1();
                break;
            case 2:
                Task2();
                break;
            case 3:
                Task3();
                break;
            case 4:
                Task4();
                break;
            default:
                Console.WriteLine("Неверный выбор.");
                break;
        }
    }
    static void Task1()
    {
        Console.WriteLine("Задание 1");

        Console.Write("Название страны -  ");
        string name = Console.ReadLine();

        Console.Write("Площадь страны -  ");
        double area = Convert.ToDouble(Console.ReadLine());

        Console.Write("Население страны -  ");
        long population = Convert.ToInt64(Console.ReadLine());

        Console.Write("Часть света -  ");
        string continent = Console.ReadLine();

        Console.Write("Название столицы -  ");
        string capital = Console.ReadLine();

        Console.Write("Население столицы -  ");
        long capitalPopulation = Convert.ToInt64(Console.ReadLine());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                SqlCommand command = new SqlCommand(
                    "INSERT INTO Countries (Name, Area, Population, Continent) " +
                    "VALUES (@Name, @Area, @Population, @Continent); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS int);",
                    connection, transaction);

                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Area", area);
                command.Parameters.AddWithValue("@Population", population);
                command.Parameters.AddWithValue("@Continent", continent);

                int countryId = Convert.ToInt32(command.ExecuteScalar());

                AddCity(connection, transaction, countryId,
                    capital, capitalPopulation, true);

                Console.Write("Количество  - ");
                int count = Convert.ToInt32(Console.ReadLine());

                for (int i = 0; i < count; i++)
                {
                    Console.Write("Название - ");
                    string cityName = Console.ReadLine();

                    Console.Write("Население  - ");
                    long cityPopulation = Convert.ToInt64(Console.ReadLine());

                    AddCity(connection, transaction, countryId,
                        cityName, cityPopulation, false);
                }

                transaction.Commit();
                Console.WriteLine("Данные добавлены");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("Ошибка - " + ex.Message);
            }
        }
    }

    static void AddCity(
        SqlConnection connection,
        SqlTransaction transaction,
        int countryId,
        string name,
        long population,
        bool isCapital)
    {
        SqlCommand command = new SqlCommand(
            "INSERT INTO Cities (CountryId, Name, Population, IsCapital) " +
            "VALUES (@CountryId, @Name, @Population, @IsCapital)",
            connection, transaction);

        command.Parameters.AddWithValue("@CountryId", countryId);
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Population", population);
        command.Parameters.AddWithValue("@IsCapital", isCapital);

        command.ExecuteNonQuery();
    }

    static void Task2()
    {
        Console.WriteLine("Задание 2");

        Console.Write("ID -  ");
        int countryId = Convert.ToInt32(Console.ReadLine());

        Console.Write("Новое название страны - ");
        string name = Console.ReadLine();

        Console.Write("Новая площадь - ");
        double area = Convert.ToDouble(Console.ReadLine());

        Console.Write("Новое население страны - ");
        long population = Convert.ToInt64(Console.ReadLine());

        Console.Write("Новая часть - ");
        string continent = Console.ReadLine();

        Console.Write("Новое название столицы - ");
        string capital = Console.ReadLine();

        Console.Write("Новое население - ");
        long capitalPopulation = Convert.ToInt64(Console.ReadLine());

        Console.Write("ID крупного города - ");
        int cityId = Convert.ToInt32(Console.ReadLine());

        Console.Write("Новое название города - ");
        string cityName = Console.ReadLine();

        Console.Write("Новое население города - ");
        long cityPopulation = Convert.ToInt64(Console.ReadLine());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand command = new SqlCommand(
                "UPDATE Countries SET Name=@Name, Area=@Area, " +
                "Population=@Population, Continent=@Continent WHERE Id=@Id",
                connection);

            command.Parameters.AddWithValue("@Id", countryId);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Area", area);
            command.Parameters.AddWithValue("@Population", population);
            command.Parameters.AddWithValue("@Continent", continent);

            command.ExecuteNonQuery();

            SqlCommand capitalCommand = new SqlCommand(
                "UPDATE Cities SET Name=@Name, Population=@Population " +
                "WHERE CountryId=@CountryId AND IsCapital=1",
                connection);

            capitalCommand.Parameters.AddWithValue("@CountryId", countryId);
            capitalCommand.Parameters.AddWithValue("@Name", capital);
            capitalCommand.Parameters.AddWithValue("@Population", capitalPopulation);

            capitalCommand.ExecuteNonQuery();

            SqlCommand cityCommand = new SqlCommand(
                "UPDATE Cities SET Name=@Name, Population=@Population " +
                "WHERE Id=@Id AND CountryId=@CountryId AND IsCapital=0",
                connection);

            cityCommand.Parameters.AddWithValue("@Id", cityId);
            cityCommand.Parameters.AddWithValue("@CountryId", countryId);
            cityCommand.Parameters.AddWithValue("@Name", cityName);
            cityCommand.Parameters.AddWithValue("@Population", cityPopulation);

            cityCommand.ExecuteNonQuery();

            Console.WriteLine("Данные обновлены");
        }
    }
    static void Task3()
    {
        Console.WriteLine("Задание 3");
        Console.WriteLine("1 - Удалить страну");
        Console.WriteLine("2 - Удалить столицу");
        Console.WriteLine("3 - Удалить крупный город");
        Console.Write("Выберите действие - ");

        int choice = Convert.ToInt32(Console.ReadLine());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            if (choice == 1)
            {
                Console.Write("ID - ");
                int countryId = Convert.ToInt32(Console.ReadLine());

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand cityCommand = new SqlCommand(
                        "DELETE FROM Cities WHERE CountryId=@Id",
                        connection, transaction);

                    cityCommand.Parameters.AddWithValue("@Id", countryId);
                    cityCommand.ExecuteNonQuery();

                    SqlCommand countryCommand = new SqlCommand(
                        "DELETE FROM Countries WHERE Id=@Id",
                        connection, transaction);

                    countryCommand.Parameters.AddWithValue("@Id", countryId);
                    countryCommand.ExecuteNonQuery();

                    transaction.Commit();
                    Console.WriteLine("Страна и её города удалены");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Ошибка - " + ex.Message);
                }
            }
            else if (choice == 2)
            {
                Console.Write("ID страны: ");
                int countryId = Convert.ToInt32(Console.ReadLine());

                SqlCommand command = new SqlCommand(
                    "DELETE FROM Cities WHERE CountryId=@Id AND IsCapital=1",
                    connection);

                command.Parameters.AddWithValue("@Id", countryId);
                command.ExecuteNonQuery();

                Console.WriteLine("Столица удалена");
            }
            else if (choice == 3)
            {
                Console.Write("ID крупного города: ");
                int cityId = Convert.ToInt32(Console.ReadLine());

                SqlCommand command = new SqlCommand(
                    "DELETE FROM Cities WHERE Id=@Id AND IsCapital=0",
                    connection);

                command.Parameters.AddWithValue("@Id", cityId);
                command.ExecuteNonQuery();

                Console.WriteLine("Город удалён");
            }
            else
            {
                Console.WriteLine("Неверный выбор");
            }
        }
    }
    static void Task4()
    {
        Console.WriteLine("Задание 4");

        ShowProcedure(
            "dbo.Top3CountriesByPopulation",
            "Топ-3 стран по населению");

        ShowProcedure(
            "dbo.Top3CapitalsByPopulation",
            "Топ-3 столиц по населению");

        ShowProcedure(
            "dbo.CountryWithMaxPopulation",
            "Страна с самым большим населением");

        ShowProcedure(
            "dbo.CityWithMaxPopulation",
            "Город с самым большим населением");
    }

    static void ShowProcedure(string procedureName, string title)
    {
        Console.WriteLine("\n" + title + ":");

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand command = new SqlCommand(procedureName, connection);
            command.CommandType = CommandType.StoredProcedure;

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i > 0)
                            Console.Write(" | ");

                        Console.Write(reader.GetName(i) + ": " + reader.GetValue(i));
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}