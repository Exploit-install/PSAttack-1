using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSPunch.PSPunchProcessing;


namespace PSPunch.Utils
{
    class Display
    {
        private static string prompt = Strings.prompt;

        public static void Output(PunchState punchState)
        {
            if (punchState.cmdComplete)
            {
                Console.WriteLine("\n" + punchState.output);
                Prompt();
            }
            int consoleTopPos = Console.CursorTop;
            Console.SetCursorPosition(prompt.Length, consoleTopPos);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(prompt.Length, consoleTopPos);
            Console.Write(punchState.displayCmd);
        }

        public static void Prompt()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(prompt);
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
    }
}
