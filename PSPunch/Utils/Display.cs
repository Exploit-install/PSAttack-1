using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSPunch.PSPunchShell;
using PSPunch.PSPunchProcessing;


namespace PSPunch.Utils
{
    class Display
    {
        public static string createPrompt(PunchState punchState)
        {
            string prompt = punchState.runspace.SessionStateProxy.Path.CurrentLocation + " #> ";
            return prompt;
        }
        

        public static void Output(PunchState punchState)
        {
            if (punchState.cmdComplete)
            {
                printPrompt(punchState);
            }
            string prompt = createPrompt(punchState);
            int consoleTopPos = Console.CursorTop;
            Console.SetCursorPosition(prompt.Length, consoleTopPos);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(prompt.Length, consoleTopPos);
            Console.Write(punchState.displayCmd);
        }

        public static void Exception(PunchState punchState, string errorMsg)
        {
            Console.ForegroundColor = PSColors.errorText;
            Console.WriteLine("ERROR: {0}\n", errorMsg);
        }

        public static void printPrompt(PunchState punchState)
        {
            string prompt = createPrompt(punchState);
            Console.ForegroundColor = PSColors.prompt;
            Console.Write(prompt);
            Console.ForegroundColor = PSColors.inputText;
        }
    }
}
