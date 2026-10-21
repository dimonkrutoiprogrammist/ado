using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace MailingListDb
{
    class Program
    {
        static void Main(string[] args)
        {
            Task1_TestDatabase();
            Task2_GetSectionPopularity();
            Task3_GetPromotionPopularity();
            Task4_GetPromotionsAndAverageAge();
            Task5_GetSectionsByGender();

            Console.ReadLine();
        }

        static void Task1_TestDatabase()
        {
            Console.WriteLine("Проверка работы");

            using (MailingListEntities db = new MailingListEntities())
            {
                Debug.Assert(db.Database.Exists(), "База данных не найдена");

                using (var transaction = db.Database.BeginTransaction())
                {
                    Buyer testBuyer = new Buyer
                    {
                        FullName = "Тестовый покупатель",
                        Gender = "Мужской",
                        BirthDate = new DateTime(1995, 1, 1),
                        City = "Тестовый город",
                        Country = "Тестовая страна"
                    };

                    db.Buyers.Add(testBuyer);
                    db.SaveChanges();

                    Buyer foundBuyer = db.Buyers.Find(testBuyer.Id);
                    Debug.Assert(foundBuyer != null, "Добавление или поиск не работает");

                    if (foundBuyer != null)
                    {
                        foundBuyer.FullName = "Измененный покупатель";
                        db.SaveChanges();

                        Debug.Assert(
                            db.Buyers.Find(testBuyer.Id).FullName == "Изменённый покупатель",
                            "Изменение записи не работает");

                        db.Buyers.Remove(foundBuyer);
                        db.SaveChanges();

                        Debug.Assert(
                            db.Buyers.Find(testBuyer.Id) == null,
                            "Удаление записи не работает");
                    }

                    transaction.Rollback();
                }

                Console.WriteLine("Проверка базы данных выполнена");
            }
        }

        static void Task2_GetSectionPopularity()
        {
            Console.WriteLine("\nПопулярность разделов рассылки");

            using (MailingListEntities db = new MailingListEntities())
            {
                var sections = db.Sections
                    .Include(section => section.Buyers)
                    .ToList()
                    .Select(section => new
                    {
                        Name = section.Name,
                        Count = section.Buyers.Count
                    })
                    .ToList();

                Console.WriteLine("Топ-3 самых популярных разделов:");

                foreach (var section in sections
                    .OrderByDescending(section => section.Count)
                    .Take(3))
                {
                    Console.WriteLine(section.Name + " | Покупателей: " + section.Count);
                }

                var mostPopular = sections
                    .OrderByDescending(section => section.Count)
                    .FirstOrDefault();

                if (mostPopular != null)
                {
                    Console.WriteLine(
                        "Самый популярный раздел: " +
                        mostPopular.Name + " | Покупателей: " + mostPopular.Count);
                }

                Console.WriteLine("Топ-3 самых непопулярных разделов:");

                foreach (var section in sections
                    .OrderBy(section => section.Count)
                    .Take(3))
                {
                    Console.WriteLine(section.Name + " | Покупателей: " + section.Count);
                }

                var leastPopular = sections
                    .OrderBy(section => section.Count)
                    .FirstOrDefault();

                if (leastPopular != null)
                {
                    Console.WriteLine(
                        "Самый непопулярный раздел: " +
                        leastPopular.Name + " | Покупателей: " + leastPopular.Count);
                }
            }
        }

        static void Task3_GetPromotionPopularity()
        {
            Console.WriteLine("\nРазделы с акционными товарами");

            using (MailingListEntities db = new MailingListEntities())
            {
                DateTime today = DateTime.Today;

                var promotions = db.Promotions
                    .Include(promotion => promotion.Product)
                    .ToList()
                    .Where(promotion =>
                        promotion.StartDate.Date <= today &&
                        promotion.EndDate.Date >= today)
                    .GroupBy(promotion => promotion.Product.SectionId)
                    .ToDictionary(group => group.Key, group => group.Count());

                var sections = db.Sections
                    .ToList()
                    .Select(section => new
                    {
                        Id = section.Id,
                        Name = section.Name,
                        Count = promotions.ContainsKey(section.Id)
                            ? promotions[section.Id]
                            : 0
                    })
                    .ToList();

                Console.WriteLine("Топ-3 разделов по количеству акционных товаров:");

                foreach (var section in sections
                    .OrderByDescending(section => section.Count)
                    .Take(3))
                {
                    Console.WriteLine(section.Name + " | Товаров: " + section.Count);
                }

                var mostPromotions = sections
                    .OrderByDescending(section => section.Count)
                    .FirstOrDefault();

                if (mostPromotions != null)
                {
                    Console.WriteLine(
                        "Больше всего акционных товаров: " +
                        mostPromotions.Name + " | Товаров: " + mostPromotions.Count);
                }

                Console.WriteLine("Топ-3 разделов с наименьшим количеством акционных товаров:");

                foreach (var section in sections
                    .OrderBy(section => section.Count)
                    .Take(3))
                {
                    Console.WriteLine(section.Name + " | Товаров: " + section.Count);
                }

                var fewestPromotions = sections
                    .OrderBy(section => section.Count)
                    .FirstOrDefault();

                if (fewestPromotions != null)
                {
                    Console.WriteLine(
                        "Меньше всего акционных товаров: " +
                        fewestPromotions.Name + " | Товаров: " + fewestPromotions.Count);
                }
            }
        }

        static void Task4_GetPromotionsAndAverageAge()
        {
            Console.WriteLine("\nАкции, заканчивающиеся через три дня");

            using (MailingListEntities db = new MailingListEntities())
            {
                DateTime today = DateTime.Today;

                List<Promotion> promotions = db.Promotions
                    .Include(promotion => promotion.Product.Section)
                    .ToList();

                foreach (Promotion promotion in promotions
                    .Where(promotion => promotion.EndDate.Date == today.AddDays(3)))
                {
                    Console.WriteLine(
                        promotion.Product.Name + " | Раздел: " +
                        promotion.Product.Section.Name + " | До: " +
                        promotion.EndDate.ToShortDateString());
                }

                List<Promotion> expiredPromotions = promotions
                    .Where(promotion => promotion.EndDate.Date < today)
                    .ToList();

                Console.WriteLine("\nАкции с закончившимся сроком");

                foreach (Promotion promotion in expiredPromotions)
                {
                    Console.WriteLine(
                        promotion.Product.Name + " | Раздел: " +
                        promotion.Product.Section.Name + " | Завершилась: " +
                        promotion.EndDate.ToShortDateString());

                    db.PromotionArchives.Add(new PromotionArchive
                    {
                        OriginalPromotionId = promotion.Id,
                        ProductName = promotion.Product.Name,
                        SectionName = promotion.Product.Section.Name,
                        StartDate = promotion.StartDate,
                        EndDate = promotion.EndDate,
                        DiscountPercent = promotion.DiscountPercent,
                        ArchivedAt = today
                    });
                }

                if (expiredPromotions.Count > 0)
                {
                    db.SaveChanges();
                    db.Promotions.RemoveRange(expiredPromotions);
                    db.SaveChanges();

                    Console.WriteLine("Закончившиеся акции перенесены в архив");
                }

                Console.WriteLine("\nСредний возраст по разделам");

                var sections = db.Sections
                    .Include(section => section.Buyers)
                    .ToList();

                foreach (Section section in sections)
                {
                    if (section.Buyers.Count > 0)
                    {
                        double averageAge = section.Buyers
                            .Average(buyer => GetAge(buyer.BirthDate));

                        Console.WriteLine(section.Name + " | Средний возраст: " + averageAge);
                    }
                }

                List<Buyer> buyers = db.Buyers.ToList();

                Console.WriteLine("\nСредний возраст по городам");

                foreach (var group in buyers.GroupBy(buyer => buyer.City))
                {
                    double averageAge = group.Average(buyer => GetAge(buyer.BirthDate));
                    Console.WriteLine(group.Key + " | Средний возраст: " + averageAge);
                }

                Console.WriteLine("\nСредний возраст по странам");

                foreach (var group in buyers.GroupBy(buyer => buyer.Country))
                {
                    double averageAge = group.Average(buyer => GetAge(buyer.BirthDate));
                    Console.WriteLine(group.Key + " | Средний возраст: " + averageAge);
                }
            }
        }

        static void Task5_GetSectionsByGender()
        {
            Console.WriteLine("\nРазделы и покупатели по полу");

            using (MailingListEntities db = new MailingListEntities())
            {
                List<Section> sections = db.Sections
                    .Include(section => section.Buyers)
                    .ToList();

                List<string> genders = db.Buyers
                    .Select(buyer => buyer.Gender)
                    .Distinct()
                    .ToList();

                foreach (string gender in genders)
                {
                    var sectionCounts = sections
                        .Select(section => new
                        {
                            Name = section.Name,
                            Count = section.Buyers.Count(buyer => buyer.Gender == gender)
                        })
                        .OrderByDescending(section => section.Count)
                        .ToList();

                    Console.WriteLine("\nПол: " + gender);

                    var mostPopular = sectionCounts.FirstOrDefault();

                    if (mostPopular != null)
                    {
                        Console.WriteLine(
                            "Самый популярный раздел: " +
                            mostPopular.Name + " | Покупателей: " + mostPopular.Count);
                    }

                    Console.WriteLine("Топ-3 разделов:");

                    foreach (var section in sectionCounts.Take(3))
                    {
                        Console.WriteLine(section.Name + " | Покупателей: " + section.Count);
                    }
                }

                Console.WriteLine("\nКоличество покупателей каждого пола:");

                foreach (var group in db.Buyers
                    .ToList()
                    .GroupBy(buyer => buyer.Gender))
                {
                    Console.WriteLine(group.Key + " | Покупателей: " + group.Count());
                }

                Console.WriteLine("\nКоличество покупателей каждого пола по странам:");

                foreach (var group in db.Buyers
                    .ToList()
                    .GroupBy(buyer => new { buyer.Gender, buyer.Country }))
                {
                    Console.WriteLine(
                        group.Key.Gender + " | " +
                        group.Key.Country + " | Покупателей: " +
                        group.Count());
                }
            }
        }

        static int GetAge(DateTime birthDate)
        {
            int age = DateTime.Today.Year - birthDate.Year;

            if (birthDate.Date > DateTime.Today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }

    public class MailingListEntities : DbContext
    {
        static MailingListEntities()
        {
            Database.SetInitializer<MailingListEntities>(null);
        }

        public MailingListEntities()
            : base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MailingListDb;Integrated Security=True;MultipleActiveResultSets=True")
        {
        }

        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionArchive> PromotionArchives { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Buyer>().ToTable("Buyers");
            modelBuilder.Entity<Section>().ToTable("Sections");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Promotion>().ToTable("Promotions");
            modelBuilder.Entity<PromotionArchive>().ToTable("PromotionArchive");

            modelBuilder.Entity<Buyer>()
                .HasMany(buyer => buyer.Sections)
                .WithMany(section => section.Buyers)
                .Map(map =>
                {
                    map.ToTable("BuyerSections");
                    map.MapLeftKey("BuyerId");
                    map.MapRightKey("SectionId");
                });

            modelBuilder.Entity<Product>()
                .Property(product => product.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Promotion>()
                .Property(promotion => promotion.DiscountPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<PromotionArchive>()
                .Property(promotion => promotion.DiscountPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Buyer>()
                .Property(buyer => buyer.BirthDate)
                .HasColumnType("date");

            modelBuilder.Entity<Promotion>()
                .Property(promotion => promotion.StartDate)
                .HasColumnType("date");

            modelBuilder.Entity<Promotion>()
                .Property(promotion => promotion.EndDate)
                .HasColumnType("date");

            modelBuilder.Entity<PromotionArchive>()
                .Property(promotion => promotion.StartDate)
                .HasColumnType("date");

            modelBuilder.Entity<PromotionArchive>()
                .Property(promotion => promotion.EndDate)
                .HasColumnType("date");

            modelBuilder.Entity<PromotionArchive>()
                .Property(promotion => promotion.ArchivedAt)
                .HasColumnType("date");

            base.OnModelCreating(modelBuilder);
        }
    }

    public class Buyer
    {
        public Buyer()
        {
            Sections = new HashSet<Section>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public virtual ICollection<Section> Sections { get; set; }
    }

    public class Section
    {
        public Section()
        {
            Buyers = new HashSet<Buyer>();
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Buyer> Buyers { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }

    public class Product
    {
        public Product()
        {
            Promotions = new HashSet<Promotion>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int SectionId { get; set; }
        public decimal Price { get; set; }

        public virtual Section Section { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
    }

    public class Promotion
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DiscountPercent { get; set; }

        public virtual Product Product { get; set; }
    }

    public class PromotionArchive
    {
        public int Id { get; set; }
        public int OriginalPromotionId { get; set; }
        public string ProductName { get; set; }
        public string SectionName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime ArchivedAt { get; set; }
    }
}