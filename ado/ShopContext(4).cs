using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;


namespace ElectronicsShopApp;

public class ShopContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(DbConfig.ConnectionString);
        }
    }
    public class OrderService
    {
        public void PlaceOrder(int customerId, Dictionary<int, int> productsToBuy)
        {
            using var context = new ShopContext();
            using var transaction = context.Database.BeginTransaction();

            try
            {
                // Шаг 1: проверяем остатки
                foreach (var kvp in productsToBuy)
                {
                    int productId = kvp.Key;
                    int quantity = kvp.Value;

                    Product product = context.Products.Find(productId);
                    if (product == null)
                        throw new Exception($"Товар с Id={productId} не найден");

                    if (product.Stock < quantity)
                        throw new Exception($"Недостаточно товара '{product.Name}': на складе {product.Stock}, нужно {quantity}");
                }

                var order = new Order
                {
                    CustomerId = customerId,
                    OrderDate = DateTime.Now,
                    TotalAmount = 0
                };
                context.Orders.Add(order);
                context.SaveChanges(); 

                
                decimal total = 0;
                foreach (var kvp in productsToBuy)
                {
                    Product product = context.Products.Find(kvp.Key);
                    int quantity = kvp.Value;

                    context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = quantity
                    });

                    product.Stock -= quantity;
                    total += product.Price * quantity;
                }

                order.TotalAmount = total;
                context.SaveChanges(); 

                transaction.Commit();
                Console.WriteLine($"Заказ №{order.Id} оформлен на сумму {total:C}");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Ошибка: {ex.Message}. Транзакция отменена.");
            }
        }
    }

}
