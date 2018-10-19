using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleDesignPatternsSampleCode.DesignPatterns.Behavioral
{
    enum State2
    {
        Locked,
        Failed,
        Unlocked
    }

    public class StateWithSwitch
    {
        public static void CreateStateWithSwitch()
        {
            string code = "1234";
            var state = State2.Locked;
            var entry = new StringBuilder();

            while (true)
            {
                switch (state)
                {
                    case State2.Locked:
                        entry.Append(ReadKey().KeyChar);

                        if (entry.ToString() == code)
                        {
                            state = State2.Unlocked;
                            break;
                        }

                        if (!code.StartsWith(entry.ToString()))
                        {
                            // the code is blatantly wrong
                            state = State2.Failed;
                        }
                        break;
                    case State2.Failed:
                        CursorLeft = 0;
                        WriteLine("FAILED");
                        entry.Clear();
                        state = State2.Locked;
                        break;
                    case State2.Unlocked:
                        CursorLeft = 0;
                        WriteLine("UNLOCKED");
                        return;
                }
            }
        }
    }
}
