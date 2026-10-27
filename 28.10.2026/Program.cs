using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace PromoMailingConnectionApp
{
    internal class Program
    {
        private const string ConnectionString = "Server=(localdb)/MSSQLLocalDB;Database=PromoMailingDb;Integrated Security=True;TrustServerCertificate=True;";
        private static SqlConnection connection = null;

        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("1. Подключиться к базе данных");
                Console.WriteLine("2. Отключиться от базы данных");
                Console.WriteLine("3. Показать всех покупателей");
                Console.WriteLine("4. Показать email всех покупателей");
                Console.WriteLine("5. Показать список разделов");
                Console.WriteLine("6. Показать все акционные товары");
                Console.WriteLine("7. Показать все города");
                Console.WriteLine("8. Показать все страны");
                Console.WriteLine("9. Показать покупателей из города");
                Console.WriteLine("10. Показать покупателей из страны");
                Console.WriteLine("11. Показать акции для страны");
                Console.WriteLine("12. Количество покупателей в каждом городе");
                Console.WriteLine("13. Количество покупателей в каждой стране");
                Console.WriteLine("14. Количество городов в каждой стране");
                Console.WriteLine("15. Среднее количество городов по всем странам");
                Console.WriteLine("16. Разделы, интересующие покупателей выбранной страны");
                Console.WriteLine("17. Акционные товары раздела за период");
                Console.WriteLine("18. Акционные товары для конкретного покупателя");
                Console.WriteLine("19. Топ-3 стран по количеству покупателей");
                Console.WriteLine("20. Лучшая страна по количеству покупателей");
                Console.WriteLine("21. Топ-3 городов по количеству покупателей");
                Console.WriteLine("22. Лучший город по количеству покупателей");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": await ConnectAsync(); break;
                    case "2": Disconnect(); break;
                    case "3":
                        await ExecuteQueryAsync("Все покупатели",
                            @"SELECT c.CustomerId, c.FullName, c.BirthDate, c.Gender, c.Email, ci.CityName, co.CountryName
                              FROM dbo.Customers AS c
                              INNER JOIN dbo.Cities AS ci ON ci.CityId = c.CityId
                              INNER JOIN dbo.Countries AS co ON co.CountryId = ci.CountryId
                              ORDER BY c.FullName;");
                        break;
                    case "4":
                        await ExecuteQueryAsync("Email всех покупателей",
                            @"SELECT Email FROM dbo.Customers ORDER BY Email;");
                        break;
                    case "5":
                        await ExecuteQueryAsync("Список разделов",
                            @"SELECT CategoryId, CategoryName FROM dbo.Categories ORDER BY CategoryName;");
                        break;
                    case "6":
                        await ExecuteQueryAsync("Все акционные товары",
                            @"SELECT pr.PromotionId, co.CountryName, cat.CategoryName, p.ProductName, p.RegularPrice, pr.DiscountPercent,
                              CAST(p.RegularPrice * (1 - pr.DiscountPercent / 100.0) AS DECIMAL(12,2)) AS PromoPrice, pr.StartDate, pr.EndDate
                              FROM dbo.Promotions AS pr
                              INNER JOIN dbo.Countries AS co ON co.CountryId = pr.CountryId
                              INNER JOIN dbo.Products AS p ON p.ProductId = pr.ProductId
                              INNER JOIN dbo.Categories AS cat ON cat.CategoryId = p.CategoryId
                              ORDER BY co.CountryName, cat.CategoryName, p.ProductName;");
                        break;
                    case "7":
                        await ExecuteQueryAsync("Все города",
                            @"SELECT ci.CityId, ci.CityName, co.CountryName
                              FROM dbo.Cities AS ci
                              INNER JOIN dbo.Countries AS co ON co.CountryId = ci.CountryId
                              ORDER BY co.CountryName, ci.CityName;");
                        break;
                    case "8":
                        await ExecuteQueryAsync("Все страны",
                            @"SELECT CountryId, CountryName FROM dbo.Countries ORDER BY CountryName;");
                        break;
                    case "9": await ShowCustomersByCityAsync(); break;
                    case "10": await ShowCustomersByCountryAsync(); break;
                    case "11": await ShowPromotionsByCountryAsync(); break;
                    case "12":
                        await ExecuteQueryAsync("Количество покупателей в каждом городе",
                            @"SELECT co.CountryName, ci.CityName, COUNT(c.CustomerId) AS CustomerCount
                              FROM dbo.Cities AS ci
                              INNER JOIN dbo.Countries AS co ON co.CountryId = ci.CountryId
                              LEFT JOIN dbo.Customers AS c ON c.CityId = ci.CityId
                              GROUP BY co.CountryName, ci.CityName
                              ORDER BY co.CountryName, ci.CityName;");
                        break;
                    case "13":
                        await ExecuteQueryAsync("Количество покупателей в каждой стране",
                            @"SELECT co.CountryName, COUNT(c.CustomerId) AS CustomerCount
                              FROM dbo.Countries AS co
                              LEFT JOIN dbo.Cities AS ci ON ci.CountryId = co.CountryId
                              LEFT JOIN dbo.Customers AS c ON c.CityId = ci.CityId
                              GROUP BY co.CountryName
                              ORDER BY co.CountryName;");
                        break;
                    case "14":
                        await ExecuteQueryAsync("Количество городов в каждой стране",
                            @"SELECT co.CountryName, COUNT(ci.CityId) AS CityCount
                              FROM dbo.Countries AS co
                              LEFT JOIN dbo.Cities AS ci ON ci.CountryId = co.CountryId
                              GROUP BY co.CountryName
                              ORDER BY co.CountryName;");
                        break;
                    case "15":
                        await ExecuteQueryAsync("Среднее количество городов по всем странам",
                            @"SELECT AVG(CAST(CityCounts.CityCount AS DECIMAL(10,2))) AS AverageCitiesPerCountry
                              FROM (
                                  SELECT co.CountryId, COUNT(ci.CityId) AS CityCount
                                  FROM dbo.Countries AS co
                                  LEFT JOIN dbo.Cities AS ci ON ci.CountryId = co.CountryId
                                  GROUP BY co.CountryId
                              ) AS CityCounts;");
                        break;
                    case "16": await ShowCustomerInterestsByCountryAsync(); break;
                    case "17": await ShowPromotionsByCategoryAndPeriodAsync(); break;
                    case "18": await ShowPromotionsForCustomerAsync(); break;
                    case "19":
                        await ExecuteQueryAsync("Топ-3 стран по количеству покупателей",
                            @"SELECT TOP 3 co.CountryName, COUNT(c.CustomerId) AS CustomerCount
                              FROM dbo.Countries AS co
                              LEFT JOIN dbo.Cities AS ci ON ci.CountryId = co.CountryId
                              LEFT JOIN dbo.Customers AS c ON c.CityId = ci.CityId
                              GROUP BY co.CountryName
                              ORDER BY COUNT(c.CustomerId) DESC, co.CountryName;");
                        break;
                    case "20":
                        await ExecuteQueryAsync("Лучшая страна по количеству покупателей",
                            @"SELECT TOP 1 co.CountryName, COUNT(c.CustomerId) AS CustomerCount
                              FROM dbo.Countries AS co
                              LEFT JOIN dbo.Cities AS ci ON ci.CountryId = co.CountryId
                              LEFT JOIN dbo.Customers AS c ON c.CityId = ci.CityId
                              GROUP BY co.CountryName
                              ORDER BY COUNT(c.CustomerId) DESC, co.CountryName;");
                        break;
                    case "21":
                        await ExecuteQueryAsync("Топ-3 городов по количеству покупателей",
                            @"SELECT TOP 3 ci.CityName, co.CountryName, COUNT(c.CustomerId) AS CustomerCount
                              FROM dbo.Cities AS ci
                              INNER JOIN dbo.Countries AS co ON co.CountryId = ci.CountryId
                              LEFT JOIN dbo.Customers AS c ON c.CityId = ci.CityId
                              GROUP BY ci.CityName, co.CountryName
                              ORDER BY COUNT(c.CustomerId) DESC, co.CountryName, ci.CityName;");
                        break;
                    case "22":
                        await ExecuteQueryAsync("Лучший город по количеству покупателей",
                            @"SELECT TOP 1 ci.CityName, co.CountryName, COUNT(c.CustomerId) AS CustomerCount
                              FROM dbo.Cities AS ci
                              INNER JOIN dbo.Countries AS co ON co.CountryId = ci.CountryId
                              LEFT JOIN dbo.Customers AS c ON c.CityId = ci.CityId
                              GROUP BY ci.CityName, co.CountryName
                              ORDER BY COUNT(c.CustomerId) DESC, co.CountryName, ci.CityName;");
                        break;
                    case "0":
                        Disconnect();
                        exit = true;
                        Console.WriteLine("Программа завершена");
                        break;
                    default:
                        Console.WriteLine("Введите номер от 0 до 22");
                        break;
                }
            }
        }

        private static async Task ConnectAsync()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                Console.WriteLine("Подключение уже открыто");
                return;
            }
            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }
            connection = new SqlConnection(ConnectionString);
            try
            {
                await connection.OpenAsync();
                Console.WriteLine("Подключение выполнено успешно");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось подключиться к базе данных");
                Console.WriteLine("Причина: " + ex.Message);
                connection.Dispose();
                connection = null;
            }
        }

        private static void Disconnect()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                Console.WriteLine("Соединение закрыто");
            }
            else
            {
                Console.WriteLine("Активного подключения нет");
            }
        }

        private static async Task ExecuteQueryAsync(string title, string query, object parameters = null)
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                Console.WriteLine("Подключитесь к базе данных");
                return;
            }
            try
            {
                var rows = (await connection.QueryAsync(query, parameters)).ToList();
                if (rows.Count == 0)
                {
                    Console.WriteLine("Записи не найдены");
                    return;
                }
                foreach (var row in rows)
                {
                    var values = (IDictionary<string, object>)row;
                    foreach (var item in values)
                    {
                        string value = item.Value == null ? "NULL" : Convert.ToString(item.Value, CultureInfo.CurrentCulture);
                        Console.Write(item.Key + ": " + value + " | ");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при выполнении запроса: " + ex.Message);
            }
        }

        private static async Task ShowCustomersByCityAsync()
        {
            Console.Write("Введите название города: ");
            string cityName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(cityName))
            {
                Console.WriteLine("Название города не может быть пустым");
                return;
            }
            await ExecuteQueryAsync(
                "Покупатели из города: " + cityName,
                @"SELECT c.CustomerId, c.FullName, c.BirthDate, c.Gender, c.Email, ci.CityName, co.CountryName
                  FROM dbo.Customers AS c
                  INNER JOIN dbo.Cities AS ci ON ci.CityId = c.CityId
                  INNER JOIN dbo.Countries AS co ON co.CountryId = ci.CountryId
                  WHERE ci.CityName = @CityName
                  ORDER BY c.FullName;",
                new { CityName = cityName });
        }

        private static async Task ShowCustomersByCountryAsync()
        {
            Console.Write("Введите название страны: ");
            string countryName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(countryName))
            {
                Console.WriteLine("Название страны не может быть пустым");
                return;
            }
            await ExecuteQueryAsync(
                "Покупатели из страны: " + countryName,
                @"SELECT c.CustomerId, c.FullName, c.BirthDate, c.Gender, c.Email, ci.CityName, co.CountryName
                  FROM dbo.Customers AS c
                  INNER JOIN dbo.Cities AS ci ON ci.CityId = c.CityId
                  INNER JOIN dbo.Countries AS co ON co.CountryId = ci.CountryId
                  WHERE co.CountryName = @CountryName
                  ORDER BY c.FullName;",
                new { CountryName = countryName });
        }

        private static async Task ShowPromotionsByCountryAsync()
        {
            Console.Write("Введите название страны: ");
            string countryName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(countryName))
            {
                Console.WriteLine("Название страны не может быть пустым");
                return;
            }
            await ExecuteQueryAsync(
                "Акции для страны: " + countryName,
                @"SELECT pr.PromotionId, co.CountryName, cat.CategoryName, p.ProductName, p.RegularPrice, pr.DiscountPercent,
                  CAST(p.RegularPrice * (1 - pr.DiscountPercent / 100.0) AS DECIMAL(12,2)) AS PromoPrice, pr.StartDate, pr.EndDate
                  FROM dbo.Promotions AS pr
                  INNER JOIN dbo.Countries AS co ON co.CountryId = pr.CountryId
                  INNER JOIN dbo.Products AS p ON p.ProductId = pr.ProductId
                  INNER JOIN dbo.Categories AS cat ON cat.CategoryId = p.CategoryId
                  WHERE co.CountryName = @CountryName
                  ORDER BY pr.StartDate, cat.CategoryName, p.ProductName;",
                new { CountryName = countryName });
        }

        private static async Task ShowCustomerInterestsByCountryAsync()
        {
            Console.Write("Введите название страны: ");
            string countryName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(countryName))
            {
                Console.WriteLine("Название страны не может быть пустым");
                return;
            }
            await ExecuteQueryAsync(
                "Разделы, интересующие покупателей страны: " + countryName,
                @"SELECT DISTINCT c.FullName, c.Email, cat.CategoryName
                  FROM dbo.Customers AS c
                  INNER JOIN dbo.Cities AS ci ON ci.CityId = c.CityId
                  INNER JOIN dbo.Countries AS co ON co.CountryId = ci.CountryId
                  INNER JOIN dbo.CustomerCategories AS cc ON cc.CustomerId = c.CustomerId
                  INNER JOIN dbo.Categories AS cat ON cat.CategoryId = cc.CategoryId
                  WHERE co.CountryName = @CountryName
                  ORDER BY c.FullName, cat.CategoryName;",
                new { CountryName = countryName });
        }

        private static async Task ShowPromotionsByCategoryAndPeriodAsync()
        {
            Console.Write("Введите название раздела: ");
            string categoryName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                Console.WriteLine("Название раздела не может быть пустым");
                return;
            }
            Console.Write("Введите начальную дату (xx.xx.xxxx): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                Console.WriteLine("Неверный формат начальной даты");
                return;
            }
            Console.Write("Введите конечную дату (xx.xx.xxxx): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                Console.WriteLine("Неверный формат конечной даты");
                return;
            }
            if (startDate.Date > endDate.Date)
            {
                Console.WriteLine("Начальная дата не может быть позже конечной");
                return;
            }
            await ExecuteQueryAsync(
                "Акционные товары раздела за выбранный период",
                @"SELECT co.CountryName, cat.CategoryName, p.ProductName, p.RegularPrice, pr.DiscountPercent,
                  CAST(p.RegularPrice * (1 - pr.DiscountPercent / 100.0) AS DECIMAL(12,2)) AS PromoPrice, pr.StartDate, pr.EndDate
                  FROM dbo.Promotions AS pr
                  INNER JOIN dbo.Countries AS co ON co.CountryId = pr.CountryId
                  INNER JOIN dbo.Products AS p ON p.ProductId = pr.ProductId
                  INNER JOIN dbo.Categories AS cat ON cat.CategoryId = p.CategoryId
                  WHERE cat.CategoryName = @CategoryName
                    AND pr.StartDate <= @EndDate
                    AND pr.EndDate >= @StartDate
                  ORDER BY pr.StartDate, p.ProductName;",
                new { CategoryName = categoryName, StartDate = startDate.Date, EndDate = endDate.Date });
        }

        private static async Task ShowPromotionsForCustomerAsync()
        {
            Console.Write("Введите ID покупателя: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("ID покупателя должен быть целым числом");
                return;
            }
            await ExecuteQueryAsync(
                "Действующие акционные товары для покупателя №" + customerId,
                @"SELECT c.CustomerId, c.FullName, co.CountryName, cat.CategoryName, p.ProductName, p.RegularPrice, pr.DiscountPercent,
                  CAST(p.RegularPrice * (1 - pr.DiscountPercent / 100.0) AS DECIMAL(12,2)) AS PromoPrice, pr.StartDate, pr.EndDate
                  FROM dbo.Customers AS c
                  INNER JOIN dbo.Cities AS ci ON ci.CityId = c.CityId
                  INNER JOIN dbo.Countries AS co ON co.CountryId = ci.CountryId
                  INNER JOIN dbo.CustomerCategories AS cc ON cc.CustomerId = c.CustomerId
                  INNER JOIN dbo.Categories AS cat ON cat.CategoryId = cc.CategoryId
                  INNER JOIN dbo.Products AS p ON p.CategoryId = cat.CategoryId
                  INNER JOIN dbo.Promotions AS pr ON pr.ProductId = p.ProductId AND pr.CountryId = co.CountryId
                  WHERE c.CustomerId = @CustomerId
                  AND CAST(GETDATE() AS DATE) BETWEEN pr.StartDate AND pr.EndDate
                  ORDER BY cat.CategoryName, p.ProductName;",
                new { CustomerId = customerId });
        }
    }
}