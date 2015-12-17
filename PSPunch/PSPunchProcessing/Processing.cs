using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using PSPunch.PSPunchShell;
using PSPunch.Utils;

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
            return punchState;
        }

        //called when tab is pressed
        static PunchState cmdAutoComplete(PunchState punchState)
        {
            if (punchState.autocompleteSeed.Length == 0)
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
                    punchState.displayCmd = punchState.displayCmdSeed + punchState.results[punchState.loopPos].BaseObject.ToString();
                }
                catch
                {

                }
                return punchState;
            }
            punchState.cmd = punchState.autocompleteSeed;
            punchState.inLoop = true;
            punchState.cmd = "Get-Command " + punchState.cmd + "*";
            punchState = PSExec(punchState);
            if (punchState.results.Count > 0)
            {
                punchState.displayCmd = punchState.displayCmdSeed + punchState.results[punchState.loopPos].BaseObject.ToString();
            }
            return punchState;
        }

        //called when tab is pressed
        static PunchState pathAutoComplete(PunchState punchState)
        {
            punchState.pathLoop = true;
            if (punchState.autocompleteSeed.Length == 0)
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
                    punchState.displayCmd = punchState.displayCmdSeed + punchState.results[punchState.loopPos].Members["FullName"].Value.ToString();
                }
                catch
                {

                }
                return punchState;
            }
            punchState.cmd = punchState.autocompleteSeed;
            punchState.inLoop = true;
            punchState.cmd = "Get-ChildItem " + punchState.cmd +"*";
            punchState = PSExec(punchState);
            if (punchState.results.Count > 0)
            {
                punchState.displayCmd = punchState.displayCmdSeed + punchState.results[punchState.loopPos].Members["FullName"].Value.ToString();
            }
            return punchState;
        }

        //called when tab and last char is -
        static PunchState paramAutoComplete(PunchState punchState)
        {
            punchState.paramLoop = true;
            if (punchState.autocompleteSeed.Length == 0)
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
                    punchState.displayCmd = punchState.displayCmdSeed + "-" + punchState.results[punchState.loopPos].ToString();
                }
                catch
                {

                }
                return punchState;
            }
            int lastParam = punchState.displayCmd.LastIndexOf(" -");
            string paramSeed = punchState.displayCmd.Substring(lastParam).Replace(" -","");
            int firstSpace = punchState.displayCmd.IndexOf(" ");
            string paramCmd = punchState.displayCmdSeed.Substring(0, firstSpace);
            punchState.cmd = punchState.autocompleteSeed;
            punchState.inLoop = true;
            punchState.cmd = "(Get-Command " + paramCmd +").Parameters.Keys.Where({$_ -like '"+paramSeed+"*'})";
            punchState = PSExec(punchState);
            if (punchState.results.Count > 0)
            {
                punchState.displayCmd = punchState.displayCmdSeed + "-" + punchState.results[punchState.loopPos].ToString();
            }
            return punchState;
        }

        // called when up or down is entered
        static PunchState history(PunchState punchState)
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

                    if ((punchState.loopPos + 1) > (punchState.history.Count - 1))
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
                return history(punchState);
            }
            else if (punchState.keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine("\n");
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
            else if (punchState.keyInfo.Key == ConsoleKey.Tab)
            {
                if (punchState.autocompleteSeed == null)
                {
                    int lastSpace = punchState.displayCmd.LastIndexOf(" ");
                    if (lastSpace > 0)
                    {
                        // get the command that we're autocompleting for by looking for the last space and pipe
                        // anything after the last space we're going to try and autocomplete. Anything between the
                        // last pipe and last space we assume is a command. 
                        int lastPipe = punchState.displayCmd.Substring(0, lastSpace + 1).LastIndexOf("|");
                        punchState.autocompleteSeed = punchState.displayCmd.Substring(lastSpace);
                        if (lastSpace - lastPipe > 2)
                        {
                            punchState.displayCmdSeed = punchState.displayCmd.Substring(lastPipe+1, (lastSpace-lastPipe));
                        }
                        else
                        {
                            punchState.displayCmdSeed = punchState.displayCmd.Substring(0, lastSpace);
                        }
                        // trim leading space from command in the event of "cmd | cmd"
                        if (punchState.displayCmdSeed.IndexOf(" ").Equals(0))
                        {
                            punchState.displayCmdSeed = punchState.displayCmd.Substring(1, lastSpace);
                        }
                    }
                    else
                    {
                        punchState.autocompleteSeed = punchState.displayCmd;
                        punchState.displayCmdSeed = "";
                    }
                }

                if (punchState.autocompleteSeed.Contains(" -"))
                {
                    return paramAutoComplete(punchState);
                }
                else if (punchState.autocompleteSeed.Contains(":") || punchState.autocompleteSeed.Contains("\\") || punchState.autocompleteSeed.Contains(".\\")) {
                    return pathAutoComplete(punchState);
                }
                else
                {
                    return cmdAutoComplete(punchState);
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