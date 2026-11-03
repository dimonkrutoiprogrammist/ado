using ElectronicsShopApp;
using static ElectronicsShopApp.ShopContext;
Console.OutputEncoding = System.Text.Encoding.UTF8;
bool rabotaet = true;
while (rabotaet)
{
        Console.WriteLine("\n - - - - - Магазин электроники - - - - - -");
             Console.WriteLine("1. Добавить товар (zad1)");
                 Console.WriteLine("2. Список товаров (zad1)");
                    Console.WriteLine("3. Остаток товара (zad1)");
                        Console.WriteLine("4. Отсоединённый режим (zad2)");
                            Console.WriteLine("5. Топ-3 (30 дней) (zad3)");
                            Console.WriteLine("6. Оформить заказ (zad4)");
                        Console.WriteLine("7. Спящие клиенты (zad5)");
                Console.WriteLine("0. Выход");
    Console.Write("Выбор - - - - ");
    string vybor = Console.ReadLine();
    try
    {
        switch (vybor)
        {
            case "1":
                Console.Write("Название товара -  ");
                string imyaTovara = Console.ReadLine();
                Console.Write("Id категории - ");
                int idKategorii = int.Parse(Console.ReadLine());
                Console.Write("Цена - ");
                decimal cena = decimal.Parse(Console.ReadLine());
                Console.Write("Остаток - ");
                int ostatok = int.Parse(Console.ReadLine());

                new ProductService().AddProduct(imyaTovara, idKategorii, cena, ostatok);
                break;

            case "2":
                new ProductService().PrintAllProductsWithCategories();
                break;

            case "3":
                Console.Write("Id товара -  ");
                int idTovara = int.Parse(Console.ReadLine());
                int znachenieOstatka = await new ProductService().GetStockAsync(idTovara);
                Console.WriteLine($"Остаток товара Id={idTovara} - {znachenieOstatka}");
                break;

            case "4":
                new DisconnectedService().DemonstrateDisconnectedMode();
                break;

            case "5":
                new LinqDemo().ShowTop3BestSelling();
                break;

            case "6":
                Console.Write("Id клиента -  ");
                int idKlienta = int.Parse(Console.ReadLine());

                var korzina = new Dictionary<int, int>();
                while (true)
                {
                    Console.Write("Id товара - ");
                    int idTovaraVKorzine = int.Parse(Console.ReadLine());
                    if (idTovaraVKorzine == 0) break;

                    Console.Write("Количество -  ");
                    int kolichestvo = int.Parse(Console.ReadLine());
                    korzina[idTovaraVKorzine] = kolichestvo;
                }

                new OrderService().PlaceOrder(idKlienta, korzina);
                break;

            case "7":
                new DapperService().PrintSleepingCustomers();
                break;

            case "0":
                rabotaet = false;
                break;

            default:
                Console.WriteLine("Непонятный пункт");
                break;
        }
    }
    catch (Exception isklyuchenie)
    {
        Console.WriteLine($"Ошибка -  {isklyuchenie.Message}");
    }
}