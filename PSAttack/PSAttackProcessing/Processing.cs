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
        public static AttackState CommandProcessor(AttackState attackState)
        {
            attackState.output = null;
            if (attackState.keyInfo.Key == ConsoleKey.Backspace)
            {
                attackState.ClearLoop();
                if (attackState.displayCmd != "" && attackState.displayCmd.Length > 0)
                {
                    attackState.displayCmd = attackState.displayCmd.Remove(attackState.displayCmd.Length - 1);
                }
            }
            else if (attackState.keyInfo.Key == ConsoleKey.UpArrow || attackState.keyInfo.Key == ConsoleKey.DownArrow)
            {
                return history(attackState);
            }
            else if (attackState.keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine("\n");
                attackState.ClearLoop();
                // don't add blank lines to history
                attackState.cmd = attackState.displayCmd;
                if (attackState.cmd != "")
                {
                    attackState.history.Add(attackState.cmd);
                }
                if (attackState.cmd == "exit")
                {
                    System.Environment.Exit(0);
                }
                else if (attackState.cmd == "clear")
                {
                    Console.Clear();
                    attackState.displayCmd = "";
                    Display.printPrompt(attackState);

                }
                else if (attackState.cmd.Contains(".exe"))
                {
                    attackState.cmd = "Start-Process -NoNewWindow -Wait " + attackState.cmd;
                    attackState = Processing.PSExec(attackState);
                    Display.Output(attackState);
                }
                // assume that we just want to execute whatever makes it here.
                else
                {
                    attackState = Processing.PSExec(attackState);
                    Display.Output(attackState);
                }
                // clear out cmd related stuff from state
                attackState.ClearIO(display:true);
            }
            else if (attackState.keyInfo.Key == ConsoleKey.Tab)
            {
               return TabExpansion.Process(attackState);
            }
            else
            {
                attackState.ClearLoop();
                attackState.displayCmd += attackState.keyInfo.KeyChar;
            }
            return attackState;
        }

        // called when up or down is entered
        static AttackState history(AttackState attackState)
        {
            if (attackState.history.Count > 0)
            {
                if (attackState.loopType == null)
                {
                    attackState.loopType = "history";
                    if (attackState.loopPos == 0)
                    {
                        attackState.loopPos = attackState.history.Count;

                    }
                }
                if (attackState.keyInfo.Key == ConsoleKey.UpArrow && attackState.loopPos > 0)
                {
                    attackState.loopPos -= 1;
                    attackState.displayCmd = attackState.history[attackState.loopPos];

                }
                if (attackState.keyInfo.Key == ConsoleKey.DownArrow)
                {

                    if ((attackState.loopPos + 1) > (attackState.history.Count - 1))
                    {
                        attackState.displayCmd = "";
                    }
                    else
                    {
                        attackState.loopPos += 1;
                        attackState.displayCmd = attackState.history[attackState.loopPos];
                    }
                }
            }
            return attackState;
        }

        // Here is where we execute posh code
        public static AttackState PSExec(AttackState attackState)
        {
            using (Pipeline pipeline = attackState.runspace.CreatePipeline())
            {
                pipeline.Commands.AddScript(attackState.cmd);
                // If we're in an auto-complete loop, we want the PSObjects, not the string from the output of the command
                // TODO: clean this up
                if (attackState.loopType != null)
                {
                    pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                }
                else
                {
                    pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output); pipeline.Commands.Add("out-default");
                }
                try
                {
                    attackState.results = pipeline.Invoke();
                }
                catch (Exception e)
                {
                    attackState.results = null;
                    Display.Exception(attackState, e.Message);
                }

                pipeline.Dispose();
            }
            //Clear out command so it doesn't get echo'd out to console again.
            attackState.ClearIO();
            if (attackState.loopType == null)
            {
                attackState.cmdComplete = true;
            }
            return attackState;
        }
    }

}