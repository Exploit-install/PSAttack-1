using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using PSAttack.PSAttackShell;
using PSAttack.Utils;

namespace PSAttack.PSAttackProcessing
{
    class Processing
    {
        // This is called everytime a key is pressed.
        public static AttackState CommandProcessor(AttackState punchState)
        {
            punchState.output = null;
            if (punchState.keyInfo.Key == ConsoleKey.Backspace)
            {
                punchState.ClearLoop();
                if (punchState.displayCmd != null && punchState.displayCmd.Length > 0)
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
                    Display.printPrompt(punchState);

                }
                else if (punchState.cmd.Contains(".exe"))
                {
                    punchState.cmd = "Start-Process -NoNewWindow -Wait " + punchState.cmd;
                    punchState = Processing.PSExec(punchState);
                    Display.Output(punchState);
                }
                else if (punchState.cmd != null)
                {
                    punchState = Processing.PSExec(punchState);
                    Display.Output(punchState);
                }
                punchState.ClearIO(display:true);
            }
            else if (punchState.keyInfo.Key == ConsoleKey.Tab)
            {
               return TabExpansion.Process(punchState);
            }
            else
            {
                punchState.ClearLoop();
                punchState.displayCmd += punchState.keyInfo.KeyChar;
            }
            return punchState;
        }

        // called when up or down is entered
        static AttackState history(AttackState punchState)
        {
            if (punchState.history.Count > 0)
            {
                if (punchState.loopType == null)
                {
                    punchState.loopType = "history";
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

        // Here is where we execute posh code
        public static AttackState PSExec(AttackState punchState)
        {
            using (Pipeline pipeline = punchState.runspace.CreatePipeline())
            {
                pipeline.Commands.AddScript(punchState.cmd);
                // If we're in an auto-complete loop, we want the PSObjects, not the string from the output of the command
                // TODO: clean this up
                if (punchState.loopType != null)
                {
                    pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                }
                else
                {
                    pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output); pipeline.Commands.Add("out-default");
                }
                try
                {
                    punchState.results = pipeline.Invoke();
                }
                catch (Exception e)
                {
                    punchState.results = null;
                    Display.Exception(punchState, e.Message);
                }

                pipeline.Dispose();
            }
            //Clear out command so it doesn't get echo'd out to console again.
            punchState.ClearIO();
            if (punchState.loopType == null)
            {
                punchState.cmdComplete = true;
            }
            return punchState;
        }
    }

}