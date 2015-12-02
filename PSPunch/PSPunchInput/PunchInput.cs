using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using PSPunch.PSPunchShell;

namespace PSPunch.PSPunchInput
{
    class PunchInput
    {
        public string cmd { get; set; }
        public ConsoleKeyInfo keyInfo { get; set; }
        public int loopPos { get; set; }
        public bool inLoop { get; set; }
        public Runspace runspace { get; set; }
        public PSPunchHost host { get; set; }
        public string output { get; set; }
    }
}
