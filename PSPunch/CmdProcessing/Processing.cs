using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation.Runspaces;
using PSPunch.PSPunchShell;

namespace PSPunch.CmdProcessing
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

        public static PunchInput CommandProcessor(PunchInput punchInput)
        {
            if (punchInput.keyInfo.Key == ConsoleKey.Backspace)
            {
                if (punchInput.cmd.Length > 0)
                {
                    punchInput.cmd = punchInput.cmd.Remove(punchInput.cmd.Length - 1);
                }
            }
            else if (punchInput.keyInfo.Key == ConsoleKey.Tab)
            {
                punchInput.cmd = "Get-Command " + punchInput.cmd + "*";
            }
            else
            {
                punchInput.cmd += punchInput.keyInfo.KeyChar;
            }
            return punchInput;
        }
    }
}
