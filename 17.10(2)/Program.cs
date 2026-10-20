using System;
using System.Collections.Generic;
using System.Linq;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Area { get; set; }
    public long Population { get; set; }
    public string Continent { get; set; }

    public override string ToString()
    {
        return $"{Name} | Площадь - {Area} | Население - {Population} | Часть света - {Continent}";
    }
}

public class City
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    public string Name { get; set; }
    public long Population { get; set; }
    public bool IsCapital { get; set; }
}

class Program
{
    private static List<Country> GetCountries()
    {
        return new List<Country>
        {
            new Country { Id = 1, Name = "Россия", Area = 17098246, Population = 146000000, Continent = "Европа" },
            new Country { Id = 2, Name = "Канада", Area = 9984670, Population = 40000000, Continent = "Северная америка" },
            new Country { Id = 3, Name = "Китай", Area = 9596961, Population = 1410000000, Continent = "Азия" },
        };
    }

    private static List<City> GetCities()
    {
        return new List<City>
        {
            new City { Id = 1, CountryId = 1, Name = "Москва", Population = 13100000, IsCapital = true },
            new City { Id = 2, CountryId = 1, Name = "Питер", Population = 5600000, IsCapital = false },
            new City { Id = 3, CountryId = 1, Name = "Новосибирск", Population = 1600000, IsCapital = false },
        };
    }

    static void Main(string[] args)
    {
        Task1_Normalization();
        Task2_MethodSyntax();
        Task3_MethodSyntax();
        Task4_MethodSyntax();
    }
    static void Task1_Normalization()
    {
        var countries = GetCountries();
        var cities = GetCities();

        Console.WriteLine("Задание 1");

        foreach (var country in countries)
        {
            Console.WriteLine(country);
        }

        Console.WriteLine("\nГорода и столицы - ");
        foreach (var city in cities)
        {
            Console.WriteLine(
                $"{city.Name} | Население -  {city.Population} | Id -  {city.CountryId} | Столица -  {city.IsCapital}");
        }
    }
    static void Task2_MethodSyntax()
    {
        var countries = GetCountries();
        var cities = GetCities();

        Console.WriteLine("\nЗадание 2");

        Console.WriteLine("\nИнформация - ");
        foreach (var country in countries)
            Console.WriteLine(country);

        Console.WriteLine("\nНазвания стран - ");
        foreach (var name in countries.Select(c => c.Name))
            Console.WriteLine(name);

        Console.WriteLine("\nНазвания столиц - ");
        foreach (var name in cities.Where(c => c.IsCapital).Select(c => c.Name))
            Console.WriteLine(name);

        Console.WriteLine("\nКрупные города - ");
        int countryId = countries.First(c => c.Name == "Russia").Id;

        foreach (var city in cities.Where(c => c.CountryId == countryId && !c.IsCapital))
            Console.WriteLine(city.Name);

        Console.WriteLine("\nСтолицы с населением больше 5 миллионов - ");
        foreach (var city in cities.Where(c => c.IsCapital && c.Population > 5000000))
            Console.WriteLine(city.Name);

        Console.WriteLine("\nЕвропейские страны - ");
        foreach (var country in countries.Where(c => c.Continent == "Europe"))
            Console.WriteLine(country.Name);

        Console.WriteLine("\nСтраны с площадью больше 1 миллион - ");
        foreach (var country in countries.Where(c => c.Area > 1000000))
            Console.WriteLine(country.Name);
    }
    static void Task3_MethodSyntax()
    {
        var countries = GetCountries();
        var cities = GetCities();

        Console.WriteLine("\nЗадание 3");

        Console.WriteLine("\nСтолицы с буквой a и p - ");
        foreach (var city in cities.Where(c =>
            c.IsCapital &&
            c.Name.ToLower().Contains("a") &&
            c.Name.ToLower().Contains("p")))
        {
            Console.WriteLine(city.Name);
        }

        Console.WriteLine("\nСтолицы с буквой вначале K - ");
        foreach (var city in cities.Where(c =>
            c.IsCapital &&
            c.Name.StartsWith("K", StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine(city.Name);
        }

        Console.WriteLine("\nСтраны с площадью от 300000 до 1000000 - ");
        foreach (var country in countries.Where(c =>
            c.Area >= 300000 && c.Area <= 1000000))
        {
            Console.WriteLine(country.Name);
        }

        Console.WriteLine("\nСтраны с населением больше 100 миллионов - ");
        foreach (var country in countries.Where(c => c.Population > 100000000))
        {
            Console.WriteLine(country.Name);
        }
    }
    static void Task4_MethodSyntax()
    {
        var countries = GetCountries();
        var cities = GetCities();

        Console.WriteLine("\nЗадание 4");

        Console.WriteLine("\nТоп 5 стран по площади - ");
        foreach (var country in countries.OrderByDescending(c => c.Area).Take(5))
            Console.WriteLine($"{country.Name}: {country.Area}");

        Console.WriteLine("\nТоп 5 столиц по населению - ");
        foreach (var city in cities.Where(c => c.IsCapital)
                                   .OrderByDescending(c => c.Population)
                                   .Take(5))
            Console.WriteLine($"{city.Name}: {city.Population}");

        Console.WriteLine("\nСтрана с самой большой площадью - ");
        Console.WriteLine(countries.OrderByDescending(c => c.Area).First().Name);

        Console.WriteLine("\nСтолица с самым большим населением - ");
        Console.WriteLine(cities.Where(c => c.IsCapital)
                                .OrderByDescending(c => c.Population)
                                .First().Name);

        Console.WriteLine("\nЕвропейская страна с самой маленькой площадью - ");
        Console.WriteLine(countries.Where(c => c.Continent == "Europe")
                                   .OrderBy(c => c.Area)
                                   .First().Name);

        Console.WriteLine("\nСредняя площадь европейских стран - ");
        Console.WriteLine(countries.Where(c => c.Continent == "Europe")
                                   .Average(c => c.Area));

        Console.WriteLine("\nТоп 3 города России по населению - ");
        int russiaId = countries.First(c => c.Name == "Russia").Id;

        foreach (var city in cities.Where(c => c.CountryId == russiaId)
                                   .OrderByDescending(c => c.Population)
                                   .Take(3))
            Console.WriteLine($"{city.Name}: {city.Population}");

        Console.WriteLine("\nОбщее количество стран - ");
        Console.WriteLine(countries.Count());

        Console.WriteLine("\nЧасть света с максимальным количеством стран - ");
        var continent = countries.GroupBy(c => c.Continent)
                                 .OrderByDescending(g => g.Count())
                                 .First();

        Console.WriteLine(continent.Key);

        Console.WriteLine("\nКоличество стран в каждой части света - ");
        foreach (var group in countries.GroupBy(c => c.Continent).OrderBy(g => g.Key))
            Console.WriteLine($"{group.Key}: {group.Count()}");
    }
}