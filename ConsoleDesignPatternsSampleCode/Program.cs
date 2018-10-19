using ConsoleDesignPatternsSampleCode.DesignPatterns.Behavioral;
using ConsoleDesignPatternsSampleCode.DesignPatterns.Creational;
using ConsoleDesignPatternsSampleCode.DesignPatterns.Structural;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleDesignPatternsSampleCode.Program;

namespace ConsoleDesignPatternsSampleCode
{
    class Program
    {
        #region Single Responsibility Principle

        public class Journal
        {
            private readonly List<string> _entries = new List<string>();
            private static int _entryId = 0;

            public int AddEntry(string Text)
            {
                _entries.Add($"{++_entryId}: {Text}");
                return _entryId; // memento
            }

            public void DelEntry(int EntryId)
            {
                _entries.RemoveAt(EntryId);
            }

            public override string ToString()
            {
                return string.Join(Environment.NewLine, _entries);
            }
        }

        public class Persistance
        {
            public void SveToFile(Journal Jrn, string FileName, bool overwrite = false)
            {
                if (overwrite || File.Exists(FileName))
                    File.WriteAllText(FileName, Jrn.ToString());
            }
        }

        #endregion

        #region Open/Close Principle

        public enum Color
        {
            Red, Green, Blue
        }

        public enum Size
        {
            Small, Medium, Large, Huge
        }

        public class Product
        {
            public string Name;
            public Color Color;
            public Size Size;

            public Product(string Name, Color Col, Size Sz)
            {
                if (string.IsNullOrWhiteSpace(Name))
                    throw new ArgumentNullException(paramName: nameof(Name));

                this.Name = Name;
                Color = Col;
                Size = Sz;
            }
        }


        // This class violates the open/close principle
        public class ProductFilter
        {
            public IEnumerable<Product> FilterByColor(IEnumerable<Product> Products, Color Col)
            {
                foreach (var p in Products)
                {
                    if (p.Color == Col)
                        yield return p;
                }
            }

            public IEnumerable<Product> FilterBySize(IEnumerable<Product> Products, Size Sz)
            {
                foreach (var p in Products)
                {
                    if (p.Size == Sz)
                        yield return p;
                }
            }

            public IEnumerable<Product> FilterBySizeAndColor(IEnumerable<Product> Products, Size Sz, Color Col)
            {
                foreach (var p in Products)
                {
                    if (p.Size == Sz && p.Color == Col)
                        yield return p;
                }
            }
        }

        // Enterprise pattern - Specification Pattern
        public interface ISpecification<T>
        {
            bool IsSatisfied(T Template);
        }

        public interface IFilter<T>
        {
            IEnumerable<T> Filter(IEnumerable<T> Items, ISpecification<T> Spec);
        }

        public class ColorSpecification : ISpecification<Product>
        {
            private readonly Color _color;

            public ColorSpecification(Color Col)
            {
                _color = Col;
            }

            public bool IsSatisfied(Product Template)
            {
                return Template.Color == _color;
            }
        }

        public class SizeSpecification : ISpecification<Product>
        {
            private readonly Size _size;

            public SizeSpecification(Size Sz)
            {
                _size = Sz;
            }

            public bool IsSatisfied(Product Template)
            {
                return Template.Size == _size;
            }
        }

        public class AndSpecification<T> : ISpecification<T>
        {
            private ISpecification<T> _first;
            private ISpecification<T> _second;

            public AndSpecification(ISpecification<T> First, ISpecification<T> Second)
            {
                _first = First ?? throw new ArgumentNullException(paramName: nameof(First)); ;
                _second = Second ?? throw new ArgumentNullException(paramName: nameof(Second));
            }

            public bool IsSatisfied(T Template)
            {
                return _first.IsSatisfied(Template) && _second.IsSatisfied(Template);
            }
        }

        public class BetterFilter : IFilter<Product>
        {
            public IEnumerable<Product> Filter(IEnumerable<Product> Items, ISpecification<Product> Spec)
            {
                foreach (var item in Items)
                {
                    if (Spec.IsSatisfied(item))
                        yield return item;
                }
            }
        }

        #endregion

        #region Liskov Substitution Principle 

        public class Rectangle
        {
            // The virtual keyword is important otherwise the iheritance won't work for the Square
            public virtual int Width { get; set; }
            public virtual int Height { get; set; }

            public Rectangle()
            {

            }

            public Rectangle(int Width, int Height)
            {
                this.Width = Width;
                this.Height = Height;
            }

            public override string ToString()
            {
                return $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}";
            }
        }

        public class Square : Rectangle
        {
            public override int Width
            {
                set { base.Width = base.Height = value; }
            }
            public override int Height
            {
                set { base.Width = base.Height = value; }
            }
        }

        #endregion

        #region Interface Segregation Princile

        public class Document
        {

        }

        public interface IPrinter
        {
            void Print(Document D);
        }

        public interface IScanner
        {
            void Scan(Document D);
        }

        public interface IFax
        {
            void Fax(Document D);
        }

        public interface IMultiFunction : IPrinter, IScanner
        {

        }

        public class MultiFuctionDevice : IMultiFunction
        {
            private IPrinter _printer;
            private IScanner _scanner;

            public MultiFuctionDevice(IPrinter Printer, IScanner Scanner)
            {
                _printer = Printer;
                _scanner = Scanner;
            }

            public void Print(Document D)
            {
                _printer.Print(D);
            }

            public void Scan(Document D)
            {
                _scanner.Scan(D);
            }
        }

        #endregion

        #region Dependency Inversion Principle

        public enum Relationship
        {
            Parent,
            Child,
            Sibling
        }

        public class Person
        {
            public string Name;
        }

        // Avoid the low-level implementation commented below
        public interface IRelationBrowser
        {
            IEnumerable<Person> FindAllChildrenOf(string Name);
        }

        // Low-level
        public class Relationships : IRelationBrowser
        {
            private List<(Person, Relationship, Person)> _relations
                = new List<(Person, Relationship, Person)>();

            public void AddParentAndChild(Person Parent, Person Child)
            {
                _relations.Add((Parent, Relationship.Parent, Child));
                _relations.Add((Child, Relationship.Child, Parent));
            }

            public IEnumerable<Person> FindAllChildrenOf(string Name)
            {
                return _relations.Where(
                    s => s.Item1.Name.Equals("John") &&
                    s.Item2 == Relationship.Parent
                    ).Select(s => s.Item3);
            }

            // Not good: low level - this is exposed => no possible change to the data
            // structure _relations. This dependency should be avoided.
            // Better use an interface: IRelatonBrowser
            //public List<(Person, Relationship, Person)> Relations => _relations;
        }

        // High-Level
        public class Research
        {
            //public Research(Relationships Rels)
            //{
            //    var relations = Rels.Relations;
            //    foreach (var r in relations.Where(
            //        s => s.Item1.Name.Equals("John") &&
            //        s.Item2 == Relationship.Parent
            //        ))
            //    {
            //        Console.WriteLine($"John has a child called {r.Item3.Name}");
            //    }
            //}

            public Research(IRelationBrowser Browser)
            {
                foreach (var p in Browser.FindAllChildrenOf("John"))
                    Console.WriteLine($"John has a child called {p.Name}");
            }
        }

        #endregion

        /// <summary>
        /// Single Responsibility Principle
        /// </summary>
        private static void SingleRespPrinciple()
        {
            var j = new Journal();
            j.AddEntry("I studied all day!");
            j.AddEntry("I worked all day!");
            Console.WriteLine(j.ToString());

            var p = new Persistance();
            var fname = @"c:\temp\journal.txt";
            p.SveToFile(j, fname, true);

            Process.Start(fname);
        }

        /// <summary>
        /// Open/Close Principle
        /// </summary>
        private static void OpenClosePrinciple()
        {
            var apple = new Product("Apple", Color.Green, Size.Small);
            var tree = new Product("Tree", Color.Green, Size.Large);
            var house = new Product("House", Color.Blue, Size.Large);

            Product[] products = { apple, tree, house };

            var pf = new ProductFilter();
            Console.WriteLine("Green products (old):");
            foreach (var item in pf.FilterByColor(products, Color.Green))
            {
                Console.WriteLine($" - {item.Name} is green");
            }

            var bf = new BetterFilter();
            Console.WriteLine("Blue products (new):");
            foreach (var item in bf.Filter(products, new ColorSpecification(Color.Blue)))
            {
                Console.WriteLine($" - {item.Name} is blue");
            }

            Console.WriteLine("Large blue products (new):");
            foreach (var item in bf.Filter(products,
                    new AndSpecification<Product>(
                        new ColorSpecification(Color.Blue),
                        new SizeSpecification(Size.Large))))
            {
                Console.WriteLine($" - {item.Name} is blue and large");
            }
        }

        /// <summary>
        /// Liskov Substitution Principle
        /// </summary>
        private static void LiskovSubstitPrinciple()
        {
            int Area(Rectangle r) => r.Width * r.Height;

            var rc = new Rectangle(2, 6);
            Console.WriteLine($"{rc} has area {Area(rc)}");

            // Replacing the Square class with the inherited class Rectangle,
            // by virtualizing the properties Width and Height
            // we get the correct result for the Area.

            Rectangle sq = new Square
            {
                Width = 4
            };
            Console.WriteLine($"{sq} has area {Area(sq)}");
        }

        /// <summary>
        /// Dependency Inversion Principle
        /// </summary>
        private static void DepInversPrinciple()
        {
            var parent = new Person { Name = "John" };
            var child1 = new Person { Name = "Henry" };
            var child2 = new Person { Name = "Mary" };

            Relationships rs = new Relationships();
            rs.AddParentAndChild(parent, child1);
            rs.AddParentAndChild(parent, child2);

            var research = new Research(rs);
        }

        /// <summary>
        /// Builder
        /// </summary>
        /// <param name="args"></param>
        private static void Builder()
        {
            var builder = new Builder("ul");
            builder.AddChild("li", "Hello");
            builder.AddChild("li", "World");

            Console.WriteLine(builder.ToString());

            Console.WriteLine("Fluent Builder");
            builder.Clear();
            builder.AddChildFluent("li", "Hi").AddChildFluent("li", "Earth");

            Console.WriteLine(builder.ToString());

        }

        /// <summary>
        /// Faceted Builder
        /// </summary>
        /// <param name="args"></param>
        private static void FacetedBuilder()
        {
            var pb = new PersonBuilder();
            Person2 person = pb
              .Lives
                .At("123 London Road")
                .In("London")
                .WithPostcode("SW12BC")
              .Works
                .At("Fabrikam")
                .AsA("Engineer")
                .Earning(123000);

            Console.WriteLine(person);
        }

        static void Main(string[] args)
        {

            #region SOLID

            //SingleRespPrinciple();

            //OpenClosePrinciple();

            //LiskovSubstitPrinciple();

            //DepInversPrinciple();

            #endregion

            #region Creational

            //Builder();

            //FacetedBuilder();

            //var coding = new ConsoleDesignPatternsSampleCode.Exercises.Coding().CreatePerson();
            //Console.WriteLine(coding.ToString());

            // Factory Method
            //FactoryMethod.CreatePoint();

            // Abstract Factory
            //AbstractFactory.CreateFactory();

            // Prototype
            //PrototypeViaSerialization.SerializeObject();

            // Singleton
            //Singleton.CreateSingleton();

            // Singleton via DI
            //SingletonViaDependencyInjection.SingletonViaDI();

            // Monostate pattern
            //SingletonViaMonostatePattern.CreateMonostate();

            #endregion

            #region Structural

            // Adapter
            //Adapter.CreateAdapterNoCaching();

            // Adapter With Caching
            //AdapterWithCaching.CreateAdapterWithCaching();

            // Bridge
            //Bridge.CreateBridge();

            // Composite
            //Composite.CreateComposite();

            // Composite Neural Network
            //CompositeNeuralNetwork.CreateCompositeNueral();

            // Decorator
            //Decorator.CreateDecorator();

            // Adapter/Decorator
            //DecoratorWithAdapter.CreateAdapterDecorator();

            // Decorator for Multiple Inheritance
            //DecoratorMultipleInheritance.CreateDecoratorMultipleInheritance();

            // Facade
            //Facade.CreateFacade();

            // Flyweight
            //Flyweight.CreateFlyweight();

            // Proxy
            //Proxy.CreateProxy();

            #endregion

            #region Behavioral

            // Chain Of Resposibility
            //ChainOfResposibility.CreateChainOfResposibility();

            // Command
            //Command.CreateCommand();

            // Interpreter
            //Interpreter.CreteInterpreter();

            // Iterator
            //Iterator.CreateIterator();

            // Mediator
            //Mediator.CreateMediator();

            // MediatorEventBroker - uses Reactive Extensions (Rx)
            //MediatorEventBroker.CreateMediatorEventBroker();

            // Memento
            //Memento.CreateMemento();

            // Null Object
            //NullObject.CreateNullObject();

            // Observer
            //ObserverWeakEvent.CreateObserverWE();

            // State
            //StateHandMade.CreateStateHM();

            // State With Switch
            //StateWithSwitch.CreateStateWithSwitch();

            // Dynamic Strategy
            //Strategy.CreateDynamicStrategy();

            // Static Strategy
            //Strategy.CreateStaticStrategy();

            // Template Method
            //TemplateMethod.CreateTemplateMethod();

            // Visitor
            Visitor.CreateVisitor();

            #endregion
        }
    }
}
