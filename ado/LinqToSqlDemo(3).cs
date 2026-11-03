using System.ComponentModel.DataAnnotations.Schema; 
using System.ComponentModel.DataAnnotations;      

namespace ElectronicsShopApp;


[Table("Products")] 
public class ProductEntity
{
    [Key] 
    [Column("Id")]
    public int Id { get; set; }

    [Column("Name")]
    public string? Name { get; set; }

    [Column("CategoryId")]
    public int CategoryId { get; set; }

    [Column("Price")]
    public decimal Price { get; set; }

    [Column("Stock")]
    public int Stock { get; set; }
}

public class LinqDemo
{
    public void ShowTop3BestSelling()
    {
        var products = LoadProducts();
        var orderItems = LoadOrderItems();
        var orders = LoadOrders();


        DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);

        var query = from oi in orderItems
                    join o in orders on oi.OrderId equals o.Id
                    join p in products on oi.ProductId equals p.Id
                    where o.OrderDate >= thirtyDaysAgo
                    group oi by p.Name into g
                    orderby g.Sum(x => x.Quantity) descending
                    select new
                    {
                        ProductName = g.Key,
                        TotalSold = g.Sum(x => x.Quantity)
                    };

        var top3 = query.Take(3).ToList();

        Console.WriteLine("Топ-3 продаваемых товаров за 30 дней:");
        foreach (var item in top3)
        {
            Console.WriteLine($"{item.ProductName} – Продано {item.TotalSold} шт.");
        }
    }


    private List<ProductEntity> LoadProducts()
    {
        var list = new List<ProductEntity>();
        using var conn = DbHelper.CreateConnection();
        using var cmd = DbHelper.CreateCommand("SELECT Id, Name, CategoryId, Price, Stock FROM Products", conn);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ProductEntity
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"].ToString(),
                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                Price = Convert.ToDecimal(reader["Price"]),
                Stock = Convert.ToInt32(reader["Stock"])
            });
        }
        return list;
    }

    private List<(int Id, int OrderId, int ProductId, int Quantity)> LoadOrderItems()
    {
        var list = new List<(int, int, int, int)>();
        using var conn = DbHelper.CreateConnection();
        using var cmd = DbHelper.CreateCommand("SELECT Id, OrderId, ProductId, Quantity FROM OrderItems", conn);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add((
                Convert.ToInt32(reader["Id"]),
                Convert.ToInt32(reader["OrderId"]),
                Convert.ToInt32(reader["ProductId"]),
                Convert.ToInt32(reader["Quantity"])
            ));
        }
        return list;
    }

    private List<(int Id, DateTime OrderDate)> LoadOrders()
    {
        var list = new List<(int, DateTime)>();
        using var conn = DbHelper.CreateConnection();
        using var cmd = DbHelper.CreateCommand("SELECT Id, OrderDate FROM Orders", conn);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add((
                Convert.ToInt32(reader["Id"]),
                Convert.ToDateTime(reader["OrderDate"])
            ));
        }
        return list;
    }
}