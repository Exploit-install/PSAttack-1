using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.Management.Automation.Runspaces;
using PSPunch.PSPunchProcessing;
using PSPunch.Utils;

namespace PSPunch
{
    class Program
    {
        static PunchState PSInit()
        {
            //Display Loading Message
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(Strings.banner);
            Console.WriteLine("PS>Punch is loading...");
            Console.ForegroundColor = ConsoleColor.White;

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
                    string fileName = resource.Replace("PSPunch.Modules.","").Replace(".ps1.enc","");
                    string decFilename = CryptoUtils.DecryptString(fileName);
                    Console.WriteLine("Decrypting: " + decFilename);
                    Stream moduleStream = assembly.GetManifestResourceStream(resource);
                    PSPUtils.ImportModules(punchState, moduleStream);
                }
            }
            //Setup PS env
            punchState.cmd = "set-executionpolicy bypass -Scope process -Force";
            Processing.PSExec(punchState);
            punchState.cmd = "function Test-Admin { $wid = [System.Security.Principal.WindowsIdentity]::GetCurrent(); $prp = New-Object System.Security.Principal.WindowsPrincipal($wid); $adm = [System.Security.Principal.WindowsBuiltInRole]::Administrator; $prp.IsInRole($adm);}; write-host 'Is Admin: '(test-admin)";
            punchState = Processing.PSExec(punchState);

            //Setup Console
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            // Display alpha warning
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Strings.warning);

            // Display Version and build date:
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            string buildString;
            string attackDate = new StreamReader(assembly.GetManifestResourceStream("PSPunch.Resources.attackDate.txt")).ReadToEnd();
            if (attackDate.Length > 12)
            {
                buildString = "It was custom made by PS>Attack on " + attackDate + "\n"; 
            }
            else
            {
                string buildDate = new StreamReader(assembly.GetManifestResourceStream("PSPunch.Resources.BuildDate.txt")).ReadToEnd();
                buildString = "It was built on " + buildDate + "\nIf you'd like a version of PS>Punch thats even harder for AV \nto detect checkout http://github.com/jaredhaight/PSAttack \n";
            }
            Console.WriteLine(Strings.welcomeMessage, Strings.version, buildString, System.Environment.Version);
            // Display Prompt
            punchState.loopPos = 0;
            punchState.cmdComplete = false;
            Display.Prompt();

            return punchState;
        }

        static void Main(string[] args)
        {
            PunchState punchState = PSInit();
            while (true)
            {
                punchState.keyInfo = Console.ReadKey();
                punchState = Processing.CommandProcessor(punchState);
                Display.Output(punchState);
            }
        }
    }
}