using System;
using System.Collections.Generic;
using System.Linq;

public class Country
{
    public string Name { get; set; }
    public string Capital { get; set; }
    public long Population { get; set; }
    public double Area { get; set; }
    public string Continent { get; set; }

    public override string ToString()
    {
        return $"{Name} | Столица - {Capital} | Население - {Population} | Площадь -  {Area} | Часть света - {Continent}";
    }
}

class Program
{
    private static List<Country> GetCountries()
    {
        return new List<Country>
        {
            new Country { Name = "Россия", Capital = "Москва", Population = 146000000, Area = 17098246, Continent = "Европа" },
            new Country { Name = "Омерика", Capital = "Вашингтон", Population = 40000000, Area = 9984670, Continent = "Северная Америка" },
            new Country { Name = "Мексика", Capital = "Антон", Population = 1410000000, Area = 9596961, Continent = "Азия" },
        };
    }

    static void Main(string[] args)
    {
        Task1();
        Task2();
        Task3();
        Task4();
    }
    static void Task1()
    {
        var countries = GetCountries();
        var cities = GetCities();

        Console.WriteLine("Задание 1");

        Console.WriteLine("\nСтраны:");
        foreach (var country in countries)
            Console.WriteLine(country);

        Console.WriteLine("\nСтолицы и крупные города:");
        foreach (var city in cities)
        {
            string countryName = countries.First(c => c.Id == city.CountryId).Name;
            Console.WriteLine($"{city.Name} | Население: {city.Population} | Страна: {countryName} | Столица: {city.IsCapital}");
        }
    }

    static void Task2()
    {
        var countries = GetCountries();
        var cities = GetCities();

        Console.WriteLine("\nЗадание 2");

        Console.WriteLine("\nВся информация о странах:");
        foreach (var country in countries)
            Console.WriteLine(country);

        Console.WriteLine("\nНазвания стран:");
        foreach (var name in countries.Select(c => c.Name))
            Console.WriteLine(name);

        Console.WriteLine("\nНазвания столиц:");
        foreach (var name in cities.Where(c => c.IsCapital).Select(c => c.Name))
            Console.WriteLine(name);

        Console.WriteLine("\nКрупные города России:");
        int countryId = countries.First(c => c.Name == "Россия").Id;

        foreach (var city in cities.Where(c => c.CountryId == countryId && !c.IsCapital))
            Console.WriteLine($"{city.Name}: {city.Population}");

        Console.WriteLine("\nСтолицы с населением больше 5 млн:");
        foreach (var city in cities.Where(c => c.IsCapital && c.Population > 5000000))
            Console.WriteLine(city.Name);

        Console.WriteLine("\nЕвропейские страны:");
        foreach (var country in countries.Where(c => c.Continent == "Европа"))
            Console.WriteLine(country.Name);

        Console.WriteLine("\nСтраны с площадью больше 1000000:");
        foreach (var country in countries.Where(c => c.Area > 1000000))
            Console.WriteLine(country.Name);
    }
    static void Task3()
    {
        var countries = GetCountries();
        var cities = GetCities();

        Console.WriteLine("\nЗадание 3");

        Console.WriteLine("\nСтолицы с буквой a и p:");
        foreach (var city in cities.Where(c =>
            c.IsCapital &&
            c.Name.ToLower().Contains("a") &&
            c.Name.ToLower().Contains("p")))
        {
            Console.WriteLine(city.Name);
        }

        Console.WriteLine("\nСтолицы с буквы k:");
        foreach (var city in cities.Where(c =>
            c.IsCapital &&
            c.Name.StartsWith("k", StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine(city.Name);
        }

        Console.WriteLine("\nСтраны с площадью от 300000 до 1000000:");
        foreach (var country in countries.Where(c =>
            c.Area >= 300000 && c.Area <= 1000000))
        {
            Console.WriteLine(country.Name);
        }

        Console.WriteLine("\nСтраны с населением больше 100000000:");
        foreach (var country in countries.Where(c => c.Population > 100000000))
            Console.WriteLine(country.Name);
    }
    static void Task4()
    {
        var countries = GetCountries();
        var cities = GetCities();

        Console.WriteLine("\nЗадание 4");

        Console.WriteLine("\nТоп 5 стран по площади:");
        foreach (var country in countries.OrderByDescending(c => c.Area).Take(5))
            Console.WriteLine($"{country.Name}: {country.Area}");

        Console.WriteLine("\nТоп 5 столиц по населению:");
        foreach (var city in cities.Where(c => c.IsCapital)
                                   .OrderByDescending(c => c.Population)
                                   .Take(5))
            Console.WriteLine($"{city.Name}: {city.Population}");

        Console.WriteLine("\nСтрана с самой большой площадью:");
        Console.WriteLine(countries.OrderByDescending(c => c.Area).First().Name);

        Console.WriteLine("\nСтолица с самым большим количеством жителей:");
        Console.WriteLine(cities.Where(c => c.IsCapital)
                                .OrderByDescending(c => c.Population)
                                .First().Name);

        Console.WriteLine("\nСтрана с самой маленькой площадью в Европе:");
        Console.WriteLine(countries.Where(c => c.Continent == "Европа")
                                   .OrderBy(c => c.Area)
                                   .First().Name);

        Console.WriteLine("\nСредняя площадь стран в Европе:");
        Console.WriteLine(countries.Where(c => c.Continent == "Европа")
                                   .Average(c => c.Area));

        Console.WriteLine("\nТоп 3 города России по населению:");
        int russiaId = countries.First(c => c.Name == "Россия").Id;

        foreach (var city in cities.Where(c => c.CountryId == russiaId)
                                   .OrderByDescending(c => c.Population)
                                   .Take(3))
            Console.WriteLine($"{city.Name}: {city.Population}");

        Console.WriteLine("\nОбщее количество стран:");
        Console.WriteLine(countries.Count());

        Console.WriteLine("\nЧасть света с максимальным количеством стран:");
        var continent = countries.GroupBy(c => c.Continent)
                                 .OrderByDescending(g => g.Count())
                                 .First();

        Console.WriteLine(continent.Key);

        Console.WriteLine("\nКоличество стран в каждой части света:");
        foreach (var group in countries.GroupBy(c => c.Continent).OrderBy(g => g.Key))
            Console.WriteLine($"{group.Key}: {group.Count()}");
    }
}