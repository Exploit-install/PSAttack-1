using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSPunch.PSPunchProcessing
{
    class TabExpansion
    {
        public static PunchState Process(PunchState punchState)
        {
            // If we're already in an autocomplete loop, just return the next result
            if (punchState.loopType != null)
            {
                if (punchState.keyInfo.Modifiers == ConsoleModifiers.Shift)
                {
                    punchState.loopPos -= 1;
                    // loop around if we're at the beginning
                    if (punchState.loopPos < 0)
                    {
                        punchState.loopPos = punchState.results.Count - 1;
                    }
                }
                else
                {
                    punchState.loopPos += 1;
                    // loop around if we reach the end
                    if (punchState.loopPos >= punchState.results.Count)
                    {
                        punchState.loopPos = 0;
                    }
                }
                string seperator;
                switch (punchState.loopType) {
                    case "param":
                        seperator = "-";
                        break;
                    default:
                        seperator = "";
                        break;
                }
                try
                {
                    punchState.displayCmd = punchState.displayCmdSeed + seperator + punchState.results[punchState.loopPos].ToString();
                }
                catch
                {

                }
                return punchState;
            }
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
                    punchState.displayCmdSeed = punchState.displayCmd.Substring(lastPipe + 1, (lastSpace - lastPipe));
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
            if (punchState.autocompleteSeed.Length == 0)
            {
                return punchState;
            }
                
            // enter loop and route to appropriate handler
            punchState.cmd = punchState.autocompleteSeed;
            if (punchState.autocompleteSeed.Contains(" -"))
            {
                return paramAutoComplete(punchState);
            }
            else if (punchState.autocompleteSeed.Contains(":") || punchState.autocompleteSeed.Contains("\\"))
            {
                return pathAutoComplete(punchState);
            }
            else
            {
                return cmdAutoComplete(punchState);
            }
        }

        // PARAMETER AUTOCOMPLETE
        static PunchState paramAutoComplete(PunchState punchState)
        {
            punchState.loopType = "param";
            int lastParam = punchState.displayCmd.LastIndexOf(" -");
            string paramSeed = punchState.displayCmd.Substring(lastParam).Replace(" -", "");
            int firstSpace = punchState.displayCmd.IndexOf(" ");
            string paramCmd = punchState.displayCmdSeed.Substring(0, firstSpace);
            punchState.cmd = "(Get-Command " + paramCmd + ").Parameters.Keys | Where{$_ -like '" + paramSeed + "*'}";
            punchState = Processing.PSExec(punchState);
            if (punchState.results.Count > 0)
            {
                punchState.displayCmd = punchState.displayCmdSeed + "-" + punchState.results[punchState.loopPos].ToString();
            }
            return punchState;
        }

        // PATH AUTOCOMPLETE
        static PunchState pathAutoComplete(PunchState punchState)
        {
            punchState.loopType = "path";
            punchState.cmd = "Get-ChildItem " + punchState.cmd + "*";
            punchState = Processing.PSExec(punchState);
            if (punchState.results.Count > 0)
            {
                punchState.displayCmd = punchState.displayCmdSeed + punchState.results[punchState.loopPos].Members["FullName"].Value.ToString();
            }
            return punchState;
        }
                
        // COMMAND AUTOCOMPLETE
        static PunchState cmdAutoComplete(PunchState punchState)
        {
            punchState.loopType = "cmd";
            punchState.cmd = "Get-Command " + punchState.cmd + "*";
            punchState = Processing.PSExec(punchState);
            if (punchState.results.Count > 0)
            {
                punchState.displayCmd = punchState.displayCmdSeed + punchState.results[punchState.loopPos].BaseObject.ToString();
            }
            return punchState;
        }

    }
}
