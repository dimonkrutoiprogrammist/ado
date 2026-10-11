
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StudentGradesApp
{
    class Program
    {
        static string connectionString;
        static string providerName;
        static string currentDatabaseName = "База не выбрана";

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (!SelectDatabase())
            {
                Console.WriteLine("Не удалось выбрать базу данных.");
                Console.ReadLine();
                return;
            }

            while (true)
            {
                Console.Clear();

                Console.WriteLine("===== ОЦЕНКИ СТУДЕНТОВ =====");
                Console.WriteLine("Текущая база: " + currentDatabaseName);
                Console.WriteLine();
                Console.WriteLine("1. Показать все оценки");
                Console.WriteLine("2. Обновить оценку");
                Console.WriteLine("3. Удалить запись");
                Console.WriteLine("4. Сменить базу данных");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите действие: ");

                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowGradesAsync();
                            break;

                        case "2":
                            await UpdateGradeAsync();
                            break;

                        case "3":
                            await DeleteGradeAsync();
                            break;

                        case "4":
                            SelectDatabase();
                            break;

                        case "0":
                            return;

                        default:
                            Console.WriteLine("Нет такого пункта.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }

        // Выбор подключения к базе 
        static bool SelectDatabase()
        {
            Console.WriteLine("\nВыберите базу данных:");
            Console.WriteLine("1. StudentGrades");
            Console.WriteLine("2. StudentGrades2");
            Console.Write("> ");

            string choice = Console.ReadLine();
            string settingsName;

            if (choice == "1")
                settingsName = "StudentGrades1";
            else if (choice == "2")
                settingsName = "StudentGrades2";
            else
            {
                Console.WriteLine("Неверный выбор.");
                return false;
            }

            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings[settingsName];

            if (settings == null)
            {
                Console.WriteLine(
                    "Настройки подключения не найдены в App.config.");
                return false;
            }

            try
            {
                // Проверяем фабрика зарегистрирована
                DbProviderFactories.GetFactory(settings.ProviderName);

                connectionString = settings.ConnectionString;
                providerName = settings.ProviderName;
                currentDatabaseName = settingsName == "StudentGrades1"
                    ? "StudentGrades"
                    : "StudentGrades2";

                Console.WriteLine(
                    "Выбрана база: " + currentDatabaseName);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Ошибка выбора базы: " + ex.Message);

                return false;
            }
        }

        // Получение фабрики 
        static DbProviderFactory GetFactory()
        {
            return DbProviderFactories.GetFactory(providerName);
        }

        // Задание 3 и 7
        static async Task ShowGradesAsync()
        {
            DbProviderFactory factory = GetFactory();

            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                    throw new Exception("Не удалось создать подключение.");

                connection.ConnectionString = connectionString;

                await connection.OpenAsync();

                using (DbCommand command = factory.CreateCommand())
                {
                    if (command == null)
                        throw new Exception("Не удалось создать команду.");

                    command.Connection = connection;
                    command.CommandText =
                        "SELECT Id, StudentName, Subject, Grade " +
                        "FROM Grades ORDER BY Id";

                    List<string> rows = new List<string>();

                    // Замер выполнения запроса 
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    using (DbDataReader reader =
                        await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            rows.Add(string.Format(
                                "{0,-5} | {1,-22} | {2,-18} | {3}",
                                reader["Id"],
                                reader["StudentName"],
                                reader["Subject"],
                                reader["Grade"]));
                        }
                    }

                    stopwatch.Stop();

                    Console.WriteLine("\nСписок оценок:\n");
                    Console.WriteLine(
                        "{0,-5} | {1,-22} | {2,-18} | {3}",
                        "ID", "Студент", "Предмет", "Оценка");

                    Console.WriteLine(new string('-', 65));

                    foreach (string row in rows)
                        Console.WriteLine(row);

                    Console.WriteLine("\nВсего записей: " + rows.Count);

                    Console.WriteLine(
                        "Время выполнения запроса: {0:F3} сек.",
                        stopwatch.Elapsed.TotalSeconds);
                }
            }
        }

        // Задание 4 и 7
        static async Task UpdateGradeAsync()
        {
            Console.Write("Введите ID записи: ");

            int id;
            if (!int.TryParse(Console.ReadLine(), out id) || id <= 0)
            {
                Console.WriteLine("ID должен быть положительным числом.");
                return;
            }

            Console.Write("Введите новую оценку (1-5): ");

            int grade;
            if (!int.TryParse(Console.ReadLine(), out grade)
                || grade < 1 || grade > 5)
            {
                Console.WriteLine("Оценка должна быть от 1 до 5.");
                return;
            }

            DbProviderFactory factory = GetFactory();

            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                    throw new Exception("Не удалось создать подключение.");

                connection.ConnectionString = connectionString;

                await connection.OpenAsync();

                using (DbCommand command = factory.CreateCommand())
                {
                    if (command == null)
                        throw new Exception("Не удалось создать команду.");

                    command.Connection = connection;
                    command.CommandText =
                        "UPDATE Grades SET Grade = @Grade WHERE Id = @Id";

                    DbParameter gradeParameter = command.CreateParameter();
                    gradeParameter.ParameterName = "@Grade";
                    gradeParameter.DbType = DbType.Int32;
                    gradeParameter.Value = grade;
                    command.Parameters.Add(gradeParameter);

                    DbParameter idParameter = command.CreateParameter();
                    idParameter.ParameterName = "@Id";
                    idParameter.DbType = DbType.Int32;
                    idParameter.Value = id;
                    command.Parameters.Add(idParameter);

                    Stopwatch stopwatch = Stopwatch.StartNew();

                    int rowsAffected =
                        await command.ExecuteNonQueryAsync();

                    stopwatch.Stop();

                    if (rowsAffected > 0)
                        Console.WriteLine("Оценка успешно обновлена!");
                    else
                        Console.WriteLine("Запись с таким ID не найдена.");

                    Console.WriteLine(
                        "Время выполнения запроса: {0:F3} сек.",
                        stopwatch.Elapsed.TotalSeconds);
                }
            }
        }

        // Задание 5 и 7
        static async Task DeleteGradeAsync()
        {
            Console.Write("Введите ID записи для удаления: ");

            int id;
            if (!int.TryParse(Console.ReadLine(), out id) || id <= 0)
            {
                Console.WriteLine("ID должен быть положительным числом.");
                return;
            }

            Console.Write(
                "Удалить запись с ID " + id + "? (д/н): ");

            string answer = Console.ReadLine();

            if (answer == null ||
                !answer.Trim().Equals(
                    "д", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Удаление отменено.");
                return;
            }

            DbProviderFactory factory = GetFactory();

            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                    throw new Exception("Не удалось создать подключение.");

                connection.ConnectionString = connectionString;

                await connection.OpenAsync();

                using (DbCommand command = factory.CreateCommand())
                {
                    if (command == null)
                        throw new Exception("Не удалось создать команду.");

                    command.Connection = connection;
                    command.CommandText =
                        "DELETE FROM Grades WHERE Id = @Id";

                    DbParameter parameter = command.CreateParameter();
                    parameter.ParameterName = "@Id";
                    parameter.DbType = DbType.Int32;
                    parameter.Value = id;
                    command.Parameters.Add(parameter);

                    Stopwatch stopwatch = Stopwatch.StartNew();

                    int rowsAffected =
                        await command.ExecuteNonQueryAsync();

                    stopwatch.Stop();

                    if (rowsAffected > 0)
                        Console.WriteLine("Запись успешно удалена!");
                    else
                        Console.WriteLine("Запись с таким ID не найдена.");

                    Console.WriteLine(
                        "Время выполнения запроса: {0:F3} сек.",
                        stopwatch.Elapsed.TotalSeconds);
                }
            }
        }
    }
}
