using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSAttack.PSAttackProcessing
{
    class TabExpansion
    {
        public static AttackState Process(AttackState punchState)
        {
            if (punchState.loopType == null)
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

                // route to appropriate autcomplete handler
                if (punchState.autocompleteSeed.Contains(" -"))
                {
                    punchState = paramAutoComplete(punchState);
                }
                else if (punchState.autocompleteSeed.Contains("$"))
                {
                    punchState = variableAutoComplete(punchState);
                }
                else if (punchState.autocompleteSeed.Contains(":") || punchState.autocompleteSeed.Contains("\\"))
                {
                    punchState = pathAutoComplete(punchState);
                }
                else
                {
                    punchState = cmdAutoComplete(punchState);
                }
            }
            // If we're already in an autocomplete loop, increment loopPos appropriately
            else if (punchState.loopType != null)
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
            }

            // if we have results, format them and return them
            if (punchState.results.Count > 0)
            {
                string seperator = "";
                string result;
                switch (punchState.loopType)
                {
                    case "param":
                        seperator = "-";
                        result = punchState.results[punchState.loopPos].ToString();
                        break;
                    case "variable":
                        seperator = "$";
                        result = punchState.results[punchState.loopPos].Members["Name"].Value.ToString();
                        break;
                    case "path":
                        result = punchState.results[punchState.loopPos].Members["FullName"].Value.ToString();
                        break;
                    default:
                        result = punchState.results[punchState.loopPos].BaseObject.ToString();
                        break;
                }
                punchState.displayCmd = punchState.displayCmdSeed + seperator + result;
            }
            return punchState;
        }

        // PARAMETER AUTOCOMPLETE
        static AttackState paramAutoComplete(AttackState punchState)
        {
            punchState.loopType = "param";
            int lastParam = punchState.displayCmd.LastIndexOf(" -");
            string paramSeed = punchState.displayCmd.Substring(lastParam).Replace(" -", "");
            int firstSpace = punchState.displayCmd.IndexOf(" ");
            string paramCmd = punchState.displayCmdSeed.Substring(0, firstSpace);
            punchState.cmd = "(Get-Command " + paramCmd + ").Parameters.Keys | Where{$_ -like '" + paramSeed + "*'}";
            punchState = Processing.PSExec(punchState);
            return punchState;
        }

        // VARIABLE AUTOCOMPLETE
        static AttackState variableAutoComplete(AttackState punchState)
        {
            punchState.loopType = "variable";
            string variableSeed = punchState.autocompleteSeed.Replace("$", "");
            punchState.cmd = "Get-Variable " + variableSeed + "*";
            punchState = Processing.PSExec(punchState);
            return punchState;
        }

        // PATH AUTOCOMPLETE
        static AttackState pathAutoComplete(AttackState punchState)
        {
            punchState.loopType = "path";
            punchState.cmd = "Get-ChildItem " + punchState.autocompleteSeed + "*";
            punchState = Processing.PSExec(punchState);
            return punchState;
        }
                
        // COMMAND AUTOCOMPLETE
        static AttackState cmdAutoComplete(AttackState punchState)
        {
            punchState.loopType = "cmd";
            punchState.cmd = "Get-Command " + punchState.autocompleteSeed + "*";
            punchState = Processing.PSExec(punchState);
            return punchState;
        }

    }
}
