using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using PSPunch.PSPunchShell;
using PSPunch.PSPunchDisplay;

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
            if (!(punchState.inLoop))
            {
                punchState.cmdComplete = true;
            }
            punchState.output = ((PSPunchHostUserInterface)punchState.host.UI).Output.ToString();
            return punchState;
        }

        // This called everytime a key is pressed.
        public static PunchState CommandProcessor(PunchState punchState)
        {
            punchState.output = null;
            if (punchState.keyInfo.Key == ConsoleKey.Backspace)
            {
                punchState.ClearLoop();
                if (punchState.displayCmd.Length > 0)
                {
                    punchState.displayCmd = punchState.displayCmd.Remove(punchState.displayCmd.Length - 1);
                }
            }
            else if (punchState.keyInfo.Key == ConsoleKey.UpArrow || punchState.keyInfo.Key == ConsoleKey.DownArrow)
            {
                if (punchState.history.Count > 0)
                {
                    if (!(punchState.inLoop))
                    {
                        punchState.inLoop = true;
                        if (punchState.loopPos == 0)
                        {
                            punchState.loopPos = punchState.history.Count;

                        }
                    }
                    if (punchState.keyInfo.Key == ConsoleKey.UpArrow && punchState.loopPos > 0)
                    {
                        punchState.loopPos -= 1;
                        punchState.displayCmd = punchState.history[punchState.loopPos];
                        
                    }
                    if (punchState.keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        
                        if ((punchState.loopPos +1) > (punchState.history.Count -1))
                        {
                            punchState.displayCmd = "";
                        }
                        else
                        {
                            punchState.loopPos += 1;
                            punchState.displayCmd = punchState.history[punchState.loopPos];
                        }
                    }
                }
                return punchState;
            }
            else if (punchState.keyInfo.Key == ConsoleKey.Tab)
            {
                if (punchState.displayCmd.Length == 0)
                {
                    return punchState;
                }
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
                if (punchState.results.Count > 0)
                {
                    punchState.displayCmd = punchState.results[punchState.loopPos].BaseObject.ToString();
                }
                return punchState;
            }
            else if (punchState.keyInfo.Key == ConsoleKey.Enter)
            {
                punchState.ClearLoop();
                punchState.cmd = punchState.displayCmd;
                punchState.history.Add(punchState.cmd);
                if (punchState.cmd == "exit")
                {
                    System.Environment.Exit(0);
                }
                else if (punchState.cmd == "clear")
                {
                    Console.Clear();
                    punchState.displayCmd = "";
                    Display.Prompt();

                }
                else if (punchState.cmd != null)
                {
                    punchState = Processing.PSExec(punchState);
                    Display.Output(punchState);
                }
                punchState.ClearIO();
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
