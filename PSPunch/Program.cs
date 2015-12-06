using System;
using System.Configuration;
using System.Reflection;
using System.Management.Automation.Runspaces;
using PSPunch.CryptUtil;
using System.IO;
using System.Text;
using PSPunch.PSPunchProcessing;
using PSPunch.PSPunchDisplay;

namespace PSPunch
{
    class Program
    {
        static PunchState PSInit()
        {
            //Display Loading Message
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"
 ___  _____  ___              _    _
| _ \/ _ \ \| _ \_  _ _ _  __| |_ | |
|  _/\__ \> >  _/ || | ' \/ _| ' \|_|
|_|  |___/_/|_|  \_,_|_||_\__|_||_(_)

");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("PS>Punch is loading...");

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
                    Console.WriteLine("Decrypting: " + resource.ToString());
                    Stream moduleStream = assembly.GetManifestResourceStream(resource);
                    ImportModules(punchState, moduleStream);
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
            Console.WriteLine(@"
############################################################
#                                                          #
#    PLEASE NOTE: This is an alpha release of PS>Punch.    #
# There are plenty of bugs and not a lot of functionality. # 
#                                                          #
#         For more info view the release notes at          #
#   https://www.github.com/jaredhaight/pspunch/releases    #
#                                                          #
############################################################

");

            // Display Version and build date:
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string version ="0.1.1-alpha";
            string buildString;
            string attackDate = new StreamReader(assembly.GetManifestResourceStream("PSPunch.Resources.attackDate.txt")).ReadToEnd();
            if (attackDate.Length > 12)
            {
                buildString = "It was custom made by PS>Attack on "+attackDate;
            }
            else
            {
                string buildDate = new StreamReader(assembly.GetManifestResourceStream("PSPunch.Resources.BuildDate.txt")).ReadToEnd();
                buildString = "It was built on " + buildDate + "\nIf you'd like a version of PS>Punch thats even harder for AV \nto detect checkout http://github.com/jaredhaight/PSAttack \n";
            }
            Console.WriteLine("Welcome to PS>Punch! This is version {0}. \n{1}", version, buildString);

            // Display Prompt
            punchState.loopPos = 0;
            punchState.cmdComplete = false;
            Display.Prompt();

            return punchState;
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