using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using PSPunch.PSPunchShell;

namespace PSPunch.PSPunchInput
{
    class PunchInput
    {
        // contents of cmd are what are executed
        public string cmd { get; set; }

        // contents of displayCmd are what are shown on screen as the command
        public string displayCmd { get; set; }
        
        // key that was last pressed
        public ConsoleKeyInfo keyInfo { get; set; }
        
        // we set a loopPos for when we're in a tab-complete loop
        public int loopPos { get; set; }
        
        // are we in a loop
        public bool inLoop { get; set; }
        public Runspace runspace { get; set; }
        public PSPunchHost host { get; set; }
        
        // ouput is what's print to screen
        public string output { get; set; }
        
        // used for auto-complete loops
        public Collection<PSObject> results { get; set; }

        // set once execution of a command has completed, breaks the while loop in main.
        public bool cmdComplete { get; set; }

        public void ClearLoop()
        {
            this.inLoop = false;
            this.results = null;
            this.loopPos = 0;
        }

        public void ClearIO()
        {
            this.displayCmd = "";
            this.cmd = "";
            this.keyInfo = new ConsoleKeyInfo();
            this.cmdComplete = false;
            this.output = null;
        }
    }
}
