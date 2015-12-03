using System;
using System.Configuration;
using System.Management.Automation.Runspaces;
using PSPunch.CryptUtil;
using System.IO;
using System.Text;
using System.Reflection;
using PSPunch.PSPunchProcessing;

namespace PSPunch
{
    class Program
    {
        static PunchState PSInit()
        {

            //Setup PS Host and runspace
            PunchState punchState = new PunchState();
            punchState.host = new PSPunchHost();
            Runspace runspace = RunspaceFactory.CreateRunspace(punchState.host);
            runspace.Open();
            punchState.runspace = runspace;

            //Decrypt modules
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resources = assembly.GetManifestResourceNames();
            foreach (string resource in resources)
            {
                if (resource.Contains(".enc"))
                {
                    Stream moduleStream = assembly.GetManifestResourceStream(resource);
                    ImportModules(punchState, moduleStream);
                }
            }
            
            // Setup Console stage 1
            string prompt = "PSPUNCH! #> ";
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            //Setup PS env
            punchState.cmd = "set-executionpolicy bypass -Scope process -Force";
            Processing.PSExec(punchState);
            punchState.cmd = "function Test-Admin { $wid = [System.Security.Principal.WindowsIdentity]::GetCurrent(); $prp = New-Object System.Security.Principal.WindowsPrincipal($wid); $adm = [System.Security.Principal.WindowsBuiltInRole]::Administrator; $prp.IsInRole($adm);}; write-host 'Is Admin: '(test-admin)";
            punchState = Processing.PSExec(punchState);

            //Setup Console
            punchState.loopPos = 0;
            punchState.cmdComplete = false;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(prompt);
            Console.ForegroundColor = ConsoleColor.Yellow;

            return punchState;

        }
        static void DisplayOutput(PunchState punchState)
        {
            int consoleTopPos = Console.CursorTop;
            string prompt = "PSPUNCH! #> ";
            Console.WriteLine("\n" + punchState.output);
            consoleTopPos = Console.CursorTop;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(prompt);
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        static void ImportModules(PunchState punchState, Stream moduleStream)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader keyReader = new StreamReader(assembly.GetManifestResourceStream("PSPunch.Modules.key.txt"));
            string key = keyReader.ReadToEnd();
            MemoryStream decMem = FileTools.DecryptFile(moduleStream, key);
            punchState.cmd = Encoding.Unicode.GetString(decMem.ToArray());
            Processing.PSExec(punchState);
        }

        static void Main(string[] args)
        {
            string prompt = "PSPUNCH! #> ";
            PunchState punchState = PSInit();
            while (true)
            {
                punchState.keyInfo = Console.ReadKey();
                punchState = Processing.CommandProcessor(punchState);
                int consoleTopPos = Console.CursorTop;
                Console.SetCursorPosition(prompt.Length, consoleTopPos);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(prompt.Length, consoleTopPos);
                Console.Write(punchState.displayCmd);
            }
        }
    }
}