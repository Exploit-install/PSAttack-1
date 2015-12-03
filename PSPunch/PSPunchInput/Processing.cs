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
                // If we're in an auto-complete loop, we want the PSObjects, not the string from the output of the command
                // TODO: clean this up
                if (punchInput.inLoop)
                {
                    pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                }
                else
                {
                    pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output); pipeline.Commands.Add("out-default");
                }

                punchInput.results = pipeline.Invoke();
                pipeline.Dispose();
            }
            //Clear out command so it doesn't get echo'd out to console again.
            punchInput.ClearIO();
            punchInput.output = ((PSPunchHostUserInterface)punchInput.host.UI).Output.ToString();
            punchInput.cmdComplete = true;
            return punchInput;
        }

        public static PunchInput CommandProcessor(PunchInput punchInput)
        {
            string prompt = "PSPUNCH! #> ";
            punchInput.output = null;
            if (punchInput.keyInfo.Key == ConsoleKey.Backspace)
            {
                punchInput.ClearLoop();
                if (punchInput.displayCmd.Length > 0)
                {
                    punchInput.displayCmd = punchInput.displayCmd.Remove(punchInput.displayCmd.Length - 1);
                }
            }
            else if (punchInput.keyInfo.Key == ConsoleKey.Tab)
            {
                // setup autocomplete loop
                if (punchInput.inLoop)
                {
                    if (punchInput.keyInfo.Modifiers == ConsoleModifiers.Shift && punchInput.loopPos > 0)
                    {
                        punchInput.loopPos -= 1;
                    }
                    else if (punchInput.loopPos < punchInput.results.Count)
                    {
                        punchInput.loopPos += 1;
                    }
                    try
                    {
                        punchInput.displayCmd = punchInput.results[punchInput.loopPos].BaseObject.ToString();
                    }
                    catch
                    {

                    }
                    return punchInput;
                }
                punchInput.cmd = punchInput.displayCmd;
                punchInput.inLoop = true;
                punchInput.cmd = "Get-Command " + punchInput.cmd + "*";
                punchInput = PSExec(punchInput);
                punchInput.displayCmd = punchInput.results[punchInput.loopPos].BaseObject.ToString();
            }
            else if (punchInput.keyInfo.Key == ConsoleKey.Enter)
            {
                punchInput.ClearLoop();
                punchInput.cmd = punchInput.displayCmd;
                if (punchInput.cmd == "exit")
                {
                    System.Environment.Exit(0);
                }
                else if (punchInput.cmd == "clear")
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(prompt);
                    Console.ForegroundColor = ConsoleColor.Yellow;

                }
                else if (punchInput.cmd != null)
                {
                    punchInput = Processing.PSExec(punchInput);
                    int consoleTopPos = Console.CursorTop;
                    Console.WriteLine("\n" + punchInput.output);
                    consoleTopPos = Console.CursorTop;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(prompt);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
            }
            else
            {
                punchInput.ClearLoop();
                punchInput.displayCmd += punchInput.keyInfo.KeyChar;
            }
            return punchInput;
        }
    }
}
