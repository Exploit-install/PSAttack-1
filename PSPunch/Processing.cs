using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation.Runspaces;
using PSPunch.PSPunchShell;

namespace PSPunch
{
    class Processing
    {
        
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

        public static string CommandProcessor(string cmd, ConsoleKeyInfo cmdKey)
        {
            if (cmdKey.Key == ConsoleKey.Backspace)
            {
                if (cmd.Length > 0)
                {
                    cmd = cmd.Remove(cmd.Length - 1);
                }
            }
            else if (cmdKey.Key == ConsoleKey.Tab)
            {
                cmd = "Get-Command " + cmd + "*";
            }
            else
            {
                cmd += cmdKey.KeyChar;
            }
            return cmd;
        }
    }
}
