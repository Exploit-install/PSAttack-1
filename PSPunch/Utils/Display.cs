using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSPunch.PSPunchShell;
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
            Console.ForegroundColor = PSColors.prompt;
            Console.Write(prompt);
            Console.ForegroundColor = PSColors.cmdText;
        }
    }
}
