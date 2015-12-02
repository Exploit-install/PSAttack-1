using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using PSPunch.PSPunchShell;
using System.Collections.ObjectModel;

namespace PSPunch.PSPunchInput
{
    class Processing
    {
        public static PunchInput PSExec(PunchInput punchInput)
        {
            using (Pipeline pipeline = punchInput.runspace.CreatePipeline())
            {
                pipeline.Commands.AddScript(punchInput.cmd);
                pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output); pipeline.Commands.Add("out-default");
                pipeline.Invoke();
                pipeline.Dispose();
            }
            //Clear out command so it doesn't get echo'd out to console again.
            punchInput.cmd = "";
            punchInput.output = ((PSPunchHostUserInterface)punchInput.host.UI).Output.ToString();
            return punchInput;
        }

        public static PunchInput CommandProcessor(PunchInput punchInput)
        {
            punchInput.output = null;
            if (punchInput.keyInfo.Key == ConsoleKey.Backspace)
            {
                if (punchInput.cmd.Length > 0)
                {
                    punchInput.cmd = punchInput.cmd.Remove(punchInput.cmd.Length - 1);
                }
            }
            else if (punchInput.keyInfo.Key == ConsoleKey.Tab)
            {
                string origInput = punchInput.cmd;
                punchInput.cmd = "Get-Command " + punchInput.cmd + "*";
                punchInput = PSExec(punchInput);
                punchInput.cmd = origInput;
            }
            else
            {
                punchInput.cmd += punchInput.keyInfo.KeyChar;
            }
            return punchInput;
        }
    }
}
