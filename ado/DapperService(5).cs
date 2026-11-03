using Dapper;
using Microsoft.Data.SqlClient;

namespace ElectronicsShopApp;


public class SleepingCustomerDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationDate { get; set; }
}

public class DapperService
{
    public List<SleepingCustomerDto> GetSleepingCustomers()
    {
        string sql = @"
            SELECT c.FullName, c.Email, c.RegistrationDate
            FROM Customers c
            LEFT JOIN Orders o ON c.Id = o.CustomerId
            WHERE o.Id IS NULL
              AND c.RegistrationDate < DATEADD(MONTH, -3, GETDATE())
            ORDER BY c.RegistrationDate";


        using var connection = new SqlConnection(DbConfig.ConnectionString);

        try
        {
            List<SleepingCustomerDto> result = connection.Query<SleepingCustomerDto>(sql).ToList();
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка Dapper-запроса: {ex.Message}");
            return new List<SleepingCustomerDto>();
        }
    }

    public void PrintSleepingCustomers()
    {
        var customers = GetSleepingCustomers();

        Console.WriteLine("«Спящие» клиенты (зарегистрированы > 3 мес. назад, 0 заказов):");
        if (customers.Count == 0)
        {
            Console.WriteLine("  (нет таких клиентов)");
            return;
        }

        foreach (var c in customers)
        {
            Console.WriteLine($"  {c.FullName} | {c.Email} | зарегистрирован: {c.RegistrationDate:dd.MM.yyyy}");
        }
    }
}