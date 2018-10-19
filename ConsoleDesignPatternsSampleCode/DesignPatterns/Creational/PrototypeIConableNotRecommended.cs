using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleDesignPatternsSampleCode.DesignPatterns.Creational
{
    // ICloneable is ill-specified for it is generally shallow copy

    public class Address : ICloneable
    {
        public readonly string StreetName;
        public int HouseNumber;

        public Address(string streetName, int houseNumber)
        {
            StreetName = streetName;
            HouseNumber = houseNumber;
        }

        public override string ToString()
        {
            return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
        }

        public object Clone()
        {
            return new Address(StreetName, HouseNumber);
        }
    }

    public class Person2Clone : ICloneable
    {
        public readonly string[] Names;
        public readonly Address Address;

        public Person2Clone(string[] names, Address address)
        {
            Names = names;
            Address = address;
        }

        public override string ToString()
        {
            return $"{nameof(Names)}: {string.Join(",", Names)}, {nameof(Address)}: {Address}";
        }

        public object Clone()
        {
            return new Person2Clone(Names, Address);
        }
    }

    public class PrototypeIConableNotRecommended
    {
        public static void CreateClone()
        {
            var john = new Person2Clone(new[] { "John", "Smith" }, new Address("London Road", 123));

            var jane = (Person2Clone)john.Clone();
            jane.Address.HouseNumber = 321; // oops, John is now at 321

            // this doesn't work
            //var jane = john;

            // but clone is typically shallow copy
            jane.Names[0] = "Jane";

            WriteLine(john);
            WriteLine(jane);
        }
    }
}
