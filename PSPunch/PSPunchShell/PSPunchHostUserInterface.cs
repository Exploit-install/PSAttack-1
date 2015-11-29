using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using System.Text;
// using System.Threading.Tasks;


namespace PSPunch.PSPunchShell
{
    class PSPunchHostUserInterface : PSHostUserInterface
    {
        private StringBuilder _sb;

        public PSPunchHostUserInterface()
        {
            _sb = new StringBuilder();
        }

        private PSPunchRawUserInterface PSPunchRawUI = new PSPunchRawUserInterface();
        public override PSHostRawUserInterface RawUI
        {
            get
            {
                return PSPunchRawUI;
            }
        }

        public override Dictionary<string, System.Management.Automation.PSObject> Prompt(string caption, string message, System.Collections.ObjectModel.Collection<FieldDescription> descriptions)
        {
            Dictionary<string, System.Management.Automation.PSObject> rtn = null;
            string msg = message + "\n";
            if (descriptions != null)
            {
                rtn = GetParameters(descriptions);
            }
            return rtn;
        }

        private Dictionary<string, System.Management.Automation.PSObject> GetParameters(System.Collections.ObjectModel.Collection<FieldDescription> descriptions)
        {
            Dictionary<string, System.Management.Automation.PSObject> rtn = new Dictionary<string, System.Management.Automation.PSObject>();
            PSParamType parm = new PSParamType();
            foreach (FieldDescription descr in descriptions)
            {
                PSParameter prm = new PSParameter();
                prm.Name = descr.Name;
                if (descr.IsMandatory)
                {
                    prm.Category = "Required";
                }
                else
                {
                    prm.Category = "Optional";
                }
                prm.DefaultValue = descr.DefaultValue;
                prm.Description = descr.HelpMessage;
                prm.Type = Type.GetType(descr.ParameterAssemblyFullName);
                if (prm.Name.ToLower() == "file" || prm.Name.ToLower() == "filename")
                {
                    prm.IsFileName = true;
                }
                if (prm.Name.ToLower() == "credential")
                {
                    prm.IsCredential = true;
                }
                parm.Properties.Add(prm);
            }
            return rtn;
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            _sb.Append(value);
        }

        public override void Write(string value)
        {
            _sb.Append(value);
        }

        public override void WriteDebugLine(string message)
        {
            _sb.AppendLine("DEBUG: " + message);
        }

        public override void WriteErrorLine(string value)
        {
            _sb.AppendLine("ERROR: " + value);
        }

        public override void WriteLine(string value)
        {
            _sb.AppendLine(value);
        }

        public override void WriteVerboseLine(string message)
        {
            _sb.AppendLine("VERBOSE: " + message);
        }

        public override void WriteWarningLine(string message)
        {
            _sb.AppendLine("WARNING: " + message);
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            return;
        }

        public string Output
        {
            get
            {
                return _sb.ToString();
            }
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            throw new NotImplementedException();
        }

        public override string ReadLine()
        {
            throw new NotImplementedException();
        }

        public override SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException();
        }
    }
}