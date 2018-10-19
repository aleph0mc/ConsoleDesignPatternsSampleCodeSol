using System;
using System.Dynamic;
using ImpromptuInterface;
using static System.Console;

namespace ConsoleDesignPatternsSampleCode.DesignPatterns.Behavioral
{
    public interface ILog
    {
        void Info(string msg);
        void Warn(string msg);
    }

    class ConsoleLog : ILog
    {
        public void Info(string msg)
        {
            WriteLine(msg);
        }

        public void Warn(string msg)
        {
            WriteLine("WARNING: " + msg);
        }
    }

    public class BankAccount3
    {
        private ILog log;
        private int balance;

        public BankAccount3(ILog log)
        {
            this.log = log;
        }

        public void Deposit(int amount)
        {
            balance += amount;
            // check for null everywhere
            log?.Info($"Deposited ${amount}, balance is now {balance}");
        }

        public void Withdraw(int amount)
        {
            if (balance >= amount)
            {
                balance -= amount;
                log?.Info($"Withdrew ${amount}, we have ${balance} left");
            }
            else
            {
                log?.Warn($"Could not withdraw ${amount} because balance is only ${balance}");
            }
        }
    }

    // Null Object design pattern
    public sealed class NullLog : ILog
    {
        public void Info(string msg)
        {

        }

        public void Warn(string msg)
        {

        }
    }

    public class Null<T> : DynamicObject where T : class
    {
        public static T Instance
        {
            get
            {
                if (!typeof(T).IsInterface)
                    throw new ArgumentException("I must be an interface type");

                return new Null<T>().ActLike<T>();
            }
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = Activator.CreateInstance(binder.ReturnType);
            return true;
        }

        private class Empty { }
    }

    public class NullObject
    {
        public static void CreateNullObject()
        {
            //var log = new ConsoleLog();
            //ILog log = null;
            //var log = new NullLog();
            var log = Null<ILog>.Instance;
            var ba = new BankAccount3(log);
            ba.Deposit(100);
            ba.Withdraw(200);
        }
    }
}
