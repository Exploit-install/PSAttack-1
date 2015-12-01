using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSPunch.PSPunchInput
{
    class PunchInput
    {
        public string cmd { get; set; }
        public ConsoleKeyInfo keyInfo { get; set; }
        public int loopPos { get; set; }
        public bool inLoop { get; set; }
    }
}
