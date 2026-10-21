using System;
using System.Data.Entity;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace CarRentalDbFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            Task1_AddInitialData();
            Task2_GetAllCars();
            Task3_FindCarByModel("Camry");
            Task4_GetCarById(1);
            Task5_GetCarsWithBrandInfo();
            Task6_AddNewCar("X5", 2023, 150.00m, 1);
            Task7_UpdateCarPrice(1, 200.00m);

            Console.ReadLine();
        }

        static void Task1_AddInitialData()
        {
            Console.WriteLine("Добавление начальных данных...");

            using (CarRentalEntities db = new CarRentalEntities())
            {
                if (!db.Brands.Any())
                {
                    db.Brands.Add(new Brand
                    {
                        Name = "Toyota",
                        Country = "Япония"
                    });

                    db.Brands.Add(new Brand
                    {
                        Name = "BMW",
                        Country = "Германия"
                    });

                    db.SaveChanges();

                    db.Cars.Add(new Car
                    {
                        Model = "Camry",
                        ReleaseYear = 2020,
                        PricePerDay = 50.00m,
                        BrandId = 1
                    });

                    db.Cars.Add(new Car
                    {
                        Model = "Corolla",
                        ReleaseYear = 2019,
                        PricePerDay = 40.00m,
                        BrandId = 1
                    });

                    db.Cars.Add(new Car
                    {
                        Model = "X5",
                        ReleaseYear = 2022,
                        PricePerDay = 100.00m,
                        BrandId = 2
                    });

                    db.SaveChanges();

                    Console.WriteLine("Данные добавлены");
                }
                else
                {
                    Console.WriteLine("База данных уже содержит записи");
                }
            }
        }

        static void Task2_GetAllCars()
        {
            Console.WriteLine("\nВсе автомобили по алфавиту");

            using (CarRentalEntities db = new CarRentalEntities())
            {
                var cars = db.Cars
                    .OrderBy(car => car.Model)
                    .ToList();

                foreach (Car car in cars)
                {
                    Console.WriteLine("Модель: " + car.Model + " | Цена за день: " + car.PricePerDay);
                }
            }
        }


        static void Task3_FindCarByModel(string modelName)
        {
            Console.WriteLine("\nПоиск машины");

            using (CarRentalEntities db = new CarRentalEntities())
            {
                var car = db.Cars
                    .FirstOrDefault(item => item.Model == modelName);

                if (car != null)
                {
                    Console.WriteLine("Модель: " + car.Model + " | Год выпуска: " + car.ReleaseYear);
                }
                else
                {
                    Console.WriteLine("Машина не найдена");
                }
            }
        }

        static void Task4_GetCarById(int id)
        {
            Console.WriteLine("\nПоиск по ID");

            using (CarRentalEntities db = new CarRentalEntities())
            {
                var car = db.Cars.Find(id);

                if (car != null)
                {
                    Console.WriteLine("Модель: " + car.Model);
                }
                else
                {
                    Console.WriteLine("Машина не найдена");
                }
            }
        }

        static void Task5_GetCarsWithBrandInfo()
        {
            Console.WriteLine("\nАвтомобили с названием марки");

            using (CarRentalEntities db = new CarRentalEntities())
            {
                var cars = db.Cars.ToList();

                foreach (Car car in cars)
                {
                    Console.WriteLine(
                        "Модель: " + car.Model +
                        " | Марка: " + car.Brand.Name +
                        " | Страна: " + car.Brand.Country);
                }
            }
        }


        static void Task6_AddNewCar(string model, int year, decimal price, int brandId)
        {
            Console.WriteLine("\nДобавление машины");

            using (CarRentalEntities db = new CarRentalEntities())
            {
                Car newCar = new Car
                {
                    Model = model,
                    ReleaseYear = year,
                    PricePerDay = price,
                    BrandId = brandId
                };

                db.Cars.Add(newCar);
                db.SaveChanges();

                Console.WriteLine("Машина успешно добавлена");
            }
        }

 
        static void Task7_UpdateCarPrice(int carId, decimal newPrice)
        {
            Console.WriteLine("\nОбновление цены машины");

            using (CarRentalEntities db = new CarRentalEntities())
            {
                var car = db.Cars.Find(carId);

                if (car != null)
                {
                    car.PricePerDay = newPrice;
                    db.SaveChanges();

                    Console.WriteLine("Цена успешно изменена");
                }
                else
                {
                    Console.WriteLine("Машина не найдена");
                }
            }
        }
    }
}