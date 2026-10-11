using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SkladApp
{
    class Program
    {
        private SqlConnection conn;

        // Главное меню
        static void Main(string[] args)
        {
            Program pr = new Program();

            while (true)
            {
                Console.Clear();

                Console.WriteLine("===== СКЛАД =====");
                Console.WriteLine("1 - Подключиться");
                Console.WriteLine("2 - Отключиться");
                Console.WriteLine("3 - Все товары");
                Console.WriteLine("4 - Все типы товаров");
                Console.WriteLine("5 - Все поставщики");
                Console.WriteLine("6 - Максимальное количество");
                Console.WriteLine("7 - Минимальное количество");
                Console.WriteLine("8 - Минимальная себестоимость");
                Console.WriteLine("9 - Максимальная себестоимость");
                Console.WriteLine("10 - Товары заданного типа");
                Console.WriteLine("11 - Товары заданного поставщика");
                Console.WriteLine("12 - Самый старый товар");
                Console.WriteLine("13 - Среднее количество по типам");
                Console.WriteLine("0 - Выход");

                Console.Write("Выберите пункт: ");
                string n = Console.ReadLine();

                switch (n)
                {
                    case "1":
                        pr.Connect();
                        break;

                    case "2":
                        pr.Disconnect();
                        break;

                    case "3":
                        pr.ShowProducts();
                        break;

                    case "4":
                        pr.ShowTypes();
                        break;

                    case "5":
                        pr.ShowSuppliers();
                        break;

                    case "6":
                        pr.MaxQuantity();
                        break;

                    case "7":
                        pr.MinQuantity();
                        break;

                    case "8":
                        pr.MinCost();
                        break;

                    case "9":
                        pr.MaxCost();
                        break;

                    case "10":
                        pr.ProductsByType();
                        break;

                    case "11":
                        pr.ProductsBySupplier();
                        break;

                    case "12":
                        pr.OldestProduct();
                        break;

                    case "13":
                        pr.AverageQuantity();
                        break;

                    case "0":
                        return;
                }
            }
        }

        // Создание подключения
        public Program()
        {
            conn = new SqlConnection();

            conn.ConnectionString =
                ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
        }

        // Подключение к БД
        public void Connect()
        {
            try
            {
                conn.Open();
                Console.WriteLine("Подключение успешно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка подключения!");
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        // Отключение от БД
        public void Disconnect()
        {
            conn.Close();
            Console.WriteLine("Соединение закрыто!");

            Console.ReadKey();
        }

        // Показать все товары
        public void ShowProducts()
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT
                    Tovary.Nazvanie,
                    TipyTovarov.Nazvanie AS Tip,
                    Postavshiki.Nazvanie AS Postavshik,
                    Tovary.Kolichestvo,
                    Tovary.Sebestoimost,
                    Tovary.DataPostavki
                  FROM Tovary
                  INNER JOIN TipyTovarov
                    ON Tovary.TipId = TipyTovarov.Id
                  INNER JOIN Postavshiki
                    ON Tovary.PostavshikId = Postavshiki.Id", conn);

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine(
                    rdr["Nazvanie"] + " | " +
                    rdr["Tip"] + " | " +
                    rdr["Postavshik"] + " | " +
                    rdr["Kolichestvo"] + " | " +
                    rdr["Sebestoimost"] + " | " +
                    rdr["DataPostavki"]);
            }

            rdr.Close();
            conn.Close();

            Console.ReadKey();
        }

        // Показать все типы
        public void ShowTypes()
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM TipyTovarov", conn);

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine(
                    rdr["Id"] + " | " +
                    rdr["Nazvanie"]);
            }

            rdr.Close();
            conn.Close();

            Console.ReadKey();
        }

        // Показать всех поставщиков
        public void ShowSuppliers()
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Postavshiki", conn);

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine(
                    rdr["Id"] + " | " +
                    rdr["Nazvanie"]);
            }

            rdr.Close();
            conn.Close();

            Console.ReadKey();
        }

        // Максимальное количество
        public void MaxQuantity()
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT TOP 1 Nazvanie, Kolichestvo
                  FROM Tovary
                  ORDER BY Kolichestvo DESC", conn);

            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                Console.WriteLine(
                    rdr["Nazvanie"] + " | " +
                    rdr["Kolichestvo"]);
            }

            rdr.Close();
            conn.Close();

            Console.ReadKey();
        }

        // Минимальное количество
        public void MinQuantity()
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT TOP 1 Nazvanie, Kolichestvo
                  FROM Tovary
                  ORDER BY Kolichestvo ASC", conn);

            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                Console.WriteLine(
                    rdr["Nazvanie"] + " | " +
                    rdr["Kolichestvo"]);
            }

            rdr.Close();
            conn.Close();

            Console.ReadKey();
        }

        // Минимальная себестоимость
        public void MinCost()
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT TOP 1 Nazvanie, Sebestoimost
                  FROM Tovary
                  ORDER BY Sebestoimost ASC", conn);

            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                Console.WriteLine(
                    rdr["Nazvanie"] + " | " +
                    rdr["Sebestoimost"]);
            }

            rdr.Close();
            conn.Close();

            Console.ReadKey();
        }

        // Максимальная себестоимость
        public void MaxCost()
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT TOP 1 Nazvanie, Sebestoimost
                  FROM Tovary
                  ORDER BY Sebestoimost DESC", conn);

            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                Console.WriteLine(
                    rdr["Nazvanie"] + " | " +
                    rdr["Sebestoimost"]);
            }

            rdr.Close();
            conn.Close();

            Console.ReadKey();
        }

        // Товары заданного типа
        public void ProductsByType()
        {
            conn.Open();

            Console.Write("Введите тип товара: ");
            string type = Console.ReadLine();

            SqlCommand cmd = new SqlCommand(
                @"SELECT Tovary.Nazvanie, Tovary.Kolichestvo
                  FROM Tovary
                  INNER JOIN TipyTovarov
                    ON Tovary.TipId = TipyTovarov.Id
                  WHERE TipyTovarov.Nazvanie = @type", conn);

            cmd.Parameters.AddWithValue("@type", type);

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine(
                    rdr["Nazvanie"] + " | " +
                    rdr["Kolichestvo"]);
            }

            rdr.Close();
            conn.Close();

            Console.ReadKey();
        }

        // Товары заданного поставщика
        public void ProductsBySupplier()
        {
            conn.Open();

            Console.Write("Введите поставщика: ");
            string supplier = Console.ReadLine();

            SqlCommand cmd = new SqlCommand(
                @"SELECT Tovary.Nazvanie, Tovary.Kolichestvo
                  FROM Tovary
                  INNER JOIN Postavshiki
                    ON Tovary.PostavshikId = Postavshiki.Id
                  WHERE Postavshiki.Nazvanie = @supplier", conn);

            cmd.Parameters.AddWithValue("@supplier", supplier);

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine(
                    rdr["Nazvanie"] + " | " +
                    rdr["Kolichestvo"]);
            }

            rdr.Close();
            conn.Close();

            Console.ReadKey();
        }

        // Самый старый товар
        public void OldestProduct()
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT TOP 1 Nazvanie, DataPostavki
                  FROM Tovary
                  ORDER BY DataPostavki ASC", conn);

            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                Console.WriteLine(
                    rdr["Nazvanie"] + " | " +
                    rdr["DataPostavki"]);
            }

            rdr.Close();
            conn.Close();

            Console.ReadKey();
        }

        // Среднее количество
        public void AverageQuantity()
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT TipyTovarov.Nazvanie,
                         AVG(Tovary.Kolichestvo) AS Srednee
                  FROM Tovary
                  INNER JOIN TipyTovarov
                    ON Tovary.TipId = TipyTovarov.Id
                  GROUP BY TipyTovarov.Nazvanie", conn);

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine(
                    rdr["Nazvanie"] + " | " +
                    rdr["Srednee"]);
            }

            rdr.Close();
            conn.Close();

            Console.ReadKey();
        }
    }
}