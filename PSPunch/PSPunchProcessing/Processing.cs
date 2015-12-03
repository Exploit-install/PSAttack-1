using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using PSPunch.PSPunchShell;
using System.Collections.ObjectModel;

namespace PSPunch.PSPunchProcessing
{
    class Processing
    {
        public static PunchState PSExec(PunchState punchState)
        {
            using (Pipeline pipeline = punchState.runspace.CreatePipeline())
            {
                pipeline.Commands.AddScript(punchState.cmd);
                // If we're in an auto-complete loop, we want the PSObjects, not the string from the output of the command
                // TODO: clean this up
                if (punchState.inLoop)
                {
                    pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                }
                else
                {
                    pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output); pipeline.Commands.Add("out-default");
                }

                punchState.results = pipeline.Invoke();
                pipeline.Dispose();
            }
            //Clear out command so it doesn't get echo'd out to console again.
            punchState.ClearIO();
            punchState.output = ((PSPunchHostUserInterface)punchState.host.UI).Output.ToString();
            punchState.cmdComplete = true;
            return punchState;
        }

        public static PunchState CommandProcessor(PunchState punchState)
        {
            string prompt = "PSPUNCH! #> ";
            punchState.output = null;
            if (punchState.keyInfo.Key == ConsoleKey.Backspace)
            {
                punchState.ClearLoop();
                if (punchState.displayCmd.Length > 0)
                {
                    punchState.displayCmd = punchState.displayCmd.Remove(punchState.displayCmd.Length - 1);
                }
            }
            else if (punchState.keyInfo.Key == ConsoleKey.Tab)
            {
                // setup autocomplete loop
                if (punchState.inLoop)
                {
                    if (punchState.keyInfo.Modifiers == ConsoleModifiers.Shift && punchState.loopPos > 0)
                    {
                        punchState.loopPos -= 1;
                    }
                    else if (punchState.loopPos < punchState.results.Count)
                    {
                        punchState.loopPos += 1;
                    }
                    try
                    {
                        punchState.displayCmd = punchState.results[punchState.loopPos].BaseObject.ToString();
                    }
                    catch
                    {

                    }
                    return punchState;
                }
                punchState.cmd = punchState.displayCmd;
                punchState.inLoop = true;
                punchState.cmd = "Get-Command " + punchState.cmd + "*";
                punchState = PSExec(punchState);
                punchState.displayCmd = punchState.results[punchState.loopPos].BaseObject.ToString();
            }
            else if (punchState.keyInfo.Key == ConsoleKey.Enter)
            {
                punchState.ClearLoop();
                punchState.cmd = punchState.displayCmd;
                if (punchState.cmd == "exit")
                {
                    System.Environment.Exit(0);
                }
                else if (punchState.cmd == "clear")
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(prompt);
                    Console.ForegroundColor = ConsoleColor.Yellow;

                }
                else if (punchState.cmd != null)
                {
                    punchState = Processing.PSExec(punchState);
                    int consoleTopPos = Console.CursorTop;
                    Console.WriteLine("\n" + punchState.output);
                    consoleTopPos = Console.CursorTop;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(prompt);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
            }
            else
            {
                punchState.ClearLoop();
                punchState.displayCmd += punchState.keyInfo.KeyChar;
            }
            return punchState;
        }
    }
}
