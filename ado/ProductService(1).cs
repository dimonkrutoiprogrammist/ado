using System.Data.Common;
namespace ElectronicsShopApp;
public class ProductService
{
    public void AddProduct(string name, int categoryId, decimal price, int stock)
    {
        string sql = @"
            INSERT INTO Products (Name, CategoryId, Price, Stock)
            VALUES (@Name, @CategoryId, @Price, @Stock)";
        using DbConnection connection = DbHelper.CreateConnection();
        using DbCommand command = DbHelper.CreateCommand(sql, connection);
        command.Parameters.Add(NewParameter(command, "@Name", name));
        command.Parameters.Add(NewParameter(command, "@CategoryId", categoryId));
        command.Parameters.Add(NewParameter(command, "@Price", price));
        command.Parameters.Add(NewParameter(command, "@Stock", stock));
        try
        {
            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine($"Добавлено строк - {rowsAffected}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении товара -  {ex.Message}");
        }

    }
    private DbParameter NewParameter(DbCommand cmd, string name, object value)
    {
        DbParameter p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value;
        return p;
    }
    public void PrintAllProductsWithCategories()
    {
        string sql = @"
        SELECT c.Name AS CategoryName, p.Name AS ProductName, p.Price, p.Stock
        FROM Products p
        INNER JOIN Categories c ON p.CategoryId = c.Id
        ORDER BY c.Name, p.Name";
        using DbConnection connection = DbHelper.CreateConnection();
        using DbCommand command = DbHelper.CreateCommand(sql, connection);
        try
        {
            connection.Open();
            using DbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string category = reader["CategoryName"].ToString();
                string product = reader["ProductName"].ToString();
                decimal price = Convert.ToDecimal(reader["Price"]);
                int stock = Convert.ToInt32(reader["Stock"]);

                Console.WriteLine($"[{category}] {product} - {price} (Остаток -  {stock} шт.)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении - {ex.Message}");
        }
    }
    public async Task<int> GetStockAsync(int productId)
    {
        string sql = "SELECT Stock FROM Products WHERE Id = @Id";
        using DbConnection connection = DbHelper.CreateConnection();
        using DbCommand command = DbHelper.CreateCommand(sql, connection);
        DbParameter p = command.CreateParameter();
        p.ParameterName = "@Id";
        p.Value = productId;
        command.Parameters.Add(p);
        try
        {
            await connection.OpenAsync();
            object result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
                return 0;
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка - {ex.Message}");
            return -1;
        }
    }
}