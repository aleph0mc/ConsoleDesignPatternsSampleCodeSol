using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleDesignPatternsSampleCode.DesignPatterns.Structural
{
    public interface IBird
    {
        void Fly();
    }

    public interface ILizard
    {
        void Crawl();
    }



    public class Bird : IBird
    {
        public int Weight { get; set; }

        public void Fly()
        {
            WriteLine($"Soaring in the sky with weight {Weight}");
        }
    }

    public class Lizard : ILizard
    {
        public int Weight { get; set; }

        public void Crawl()
        {
            WriteLine($"Crawling on the wall with weight {Weight}");
        }
    }

    public class Dragon // no multiple inheritance
    {
        private Bird bird = new Bird();
        private Lizard lizard = new Lizard();

        private int weight;

        public int Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                bird.Weight = lizard.Weight = value;
            }
        }

        public void Crawl()
        {
            lizard.Crawl();
        }

        public void Fly()
        {
            bird.Fly();
        }
    }

    public class DecoratorMultipleInheritance
    {
        public static void CreateDecoratorMultipleInheritance()
        {
            var dragon = new Dragon
            {
                Weight = 123
            };
            dragon.Crawl();
            dragon.Fly();
        }

    }
}
