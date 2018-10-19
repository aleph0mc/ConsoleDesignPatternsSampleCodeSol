using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDesignPatternsSampleCode.DesignPatterns.Creational
{
    public class Person
    {
        public string Name;

        public string Position;

        public class BuilderInfo : PersonInfoBuilder<BuilderInfo> { /* degenerate */ }

        public static BuilderInfo New => new BuilderInfo();

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Position)}: {Position}";
        }
    }

    public abstract class PersonBuilder<SELF>
      where SELF : PersonBuilder<SELF>
    {
        protected Person person = new Person();

        public Person Build()
        {
            return person;
        }
    }

    public class PersonInfoBuilder<SELF> : PersonBuilder<PersonInfoBuilder<SELF>>
      where SELF : PersonInfoBuilder<SELF>
    {
        public SELF Called(string name)
        {
            person.Name = name;
            return (SELF)this;
        }
    }

    public class PersonJobBuilder<SELF> : PersonInfoBuilder<PersonJobBuilder<SELF>>
      where SELF : PersonJobBuilder<SELF>
    {
        public SELF WorksAsA(string position)
        {
            person.Position = position;
            return (SELF)this;
        }
    }

    public class BuilderRecursiveWithGenerics
    {
        public void BuilderInheritanceDemo()
        {
            //Person.New.Called("Al").
        }
    }
}
