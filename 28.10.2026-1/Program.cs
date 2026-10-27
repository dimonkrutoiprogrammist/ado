using System;
using System.Data.SqlClient;
using Dapper;

namespace PromoMailingConnectionApp
{
    internal class Program
    {
        private const string ConnectionString = "Server=localhost;Database=PromoMailingDb;Integrated Security=True;TrustServerCertificate=True;";
        private static SqlConnection connection = null;

        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n");
                Console.WriteLine("1. Подключиться");
                Console.WriteLine("2. Отключиться");
                Console.WriteLine("3. Показать всех покупателей");
                Console.WriteLine("4. Показать email всех покупателей");
                Console.WriteLine("5. Показать список разделов");
                Console.WriteLine("6. Показать список акционных товаров");
                Console.WriteLine("7. Показать все города");
                Console.WriteLine("8. Показать все страны");
                Console.WriteLine("9. Показать покупателей из города");
                Console.WriteLine("10. Показать покупателей из страны");
                Console.WriteLine("11. Показать все акции для страны");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": Connect(); break;
                    case "2": Disconnect(); break;
                    case "3":
                        ExecuteQuery("Все покупатели",
                            "SELECT c.CustomerId, c.FullName, c.BirthDate, c.Gender, c.Email, ci.CityName, co.CountryName " +
                            "FROM dbo.Customers AS c " +
                            "INNER JOIN dbo.Cities AS ci ON ci.CityId = c.CityId " +
                            "INNER JOIN dbo.Countries AS co ON co.CountryId = ci.CountryId " +
                            "ORDER BY c.FullName;");
                        break;
                    case "4":
                        ExecuteQuery("Email всех покупателей",
                            "SELECT Email FROM dbo.Customers ORDER BY Email;");
                        break;
                    case "5":
                        ExecuteQuery("Список разделов",
                            "SELECT CategoryId, CategoryName FROM dbo.Categories ORDER BY CategoryName;");
                        break;
                    case "6":
                        ExecuteQuery("Список акционных товаров",
                            "SELECT pr.PromotionId, co.CountryName, cat.CategoryName, p.ProductName, p.RegularPrice, pr.DiscountPercent, " +
                            "CAST(p.RegularPrice * (1 - pr.DiscountPercent / 100.0) AS DECIMAL(12,2)) AS PromoPrice, pr.StartDate, pr.EndDate " +
                            "FROM dbo.Promotions AS pr " +
                            "INNER JOIN dbo.Countries AS co ON co.CountryId = pr.CountryId " +
                            "INNER JOIN dbo.Products AS p ON p.ProductId = pr.ProductId " +
                            "INNER JOIN dbo.Categories AS cat ON cat.CategoryId = p.CategoryId " +
                            "ORDER BY co.CountryName, cat.CategoryName, p.ProductName;");
                        break;
                    case "7":
                        ExecuteQuery("Все города",
                            "SELECT ci.CityId, ci.CityName, co.CountryName " +
                            "FROM dbo.Cities AS ci " +
                            "INNER JOIN dbo.Countries AS co ON co.CountryId = ci.CountryId " +
                            "ORDER BY co.CountryName, ci.CityName;");
                        break;
                    case "8":
                        ExecuteQuery("Все страны",
                            "SELECT CountryId, CountryName FROM dbo.Countries ORDER BY CountryName;");
                        break;
                    case "9":
                        ShowCustomersByCity();
                        break;
                        [file content end]