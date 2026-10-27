using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperExample
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
    }

    class Program
    {
        static string connectionString = "Server=localhost;Database=TestDB;Integrated Security=True;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            var newUser = new User
            {
                Name = "Алексей Смирнов",
                Email = "alex.smirnov@mail.ru",
                Age = 27
            };

            string insertSql = "INSERT INTO Users (Name, Email, Age) VALUES (@Name, @Email, @Age)";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int rowsAffected = db.Execute(insertSql, newUser);
                Console.WriteLine($"Вставлено строк: {rowsAffected}");
            }

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var users = db.Query<User>("SELECT * FROM Users WHERE Age > @Age", new { Age = 20 }).ToList();
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}, Имя: {user.Name}, Email: {user.Email}, Возраст: {user.Age}");
                }
            }

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var updateSql = "UPDATE Users SET Age = @Age WHERE Name = @Name";
                db.Execute(updateSql, new { Age = 28, Name = "Алексей Смирнов" });
            }

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var deleteSql = "DELETE FROM Users WHERE Name = @Name";
                db.Execute(deleteSql, new { Name = "Алексей Смирнов" });
            }
        }
    }
}