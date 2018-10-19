using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Autofac;
using static System.Console;

namespace ConsoleDesignPatternsSampleCode.DesignPatterns.Creational
{
    /// <summary>
    /// Not a singleton but used as a singleton via DI
    /// </summary>
    public class OrdinaryDatabase : IDatabase
    {
        private Dictionary<string, int> cities;

        public OrdinaryDatabase()
        {
            WriteLine("Initializing database");

            var assembly = typeof(IDatabase).Assembly.Location;

            cities = File.ReadAllLines(
              Path.Combine(
                new FileInfo(assembly).DirectoryName, "zCities.txt")
              )
              .Batch(2) // via MoreLinq
              .ToDictionary(
                list => list.ElementAt(0).Trim(),
                list => int.Parse(list.ElementAt(1)));
        }

        public int GetPopulation(string name)
        {
            return cities[name];
        }
    }


    public class SingletonViaDependencyInjection
    {
        public static void SingletonViaDI()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<OrdinaryDatabase>().As<IDatabase>().SingleInstance();

            builder.RegisterType<ConfigurableRecordFinder>();

            using (var container = builder.Build())
            {
                var rf = container.Resolve<ConfigurableRecordFinder>();
                var city = new[] { "Rome" };
                var people = rf.GetTotalPopulation(city);
                WriteLine($"{city} has population {people}");
            }
        }
    }
}
