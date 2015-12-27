using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSPunch
{
    class Strings
    {
        public static string version = "0.1.9-alpha";
        public static string windowTitle = "PSPunch!!";
        public static string banner = @"
 ___  _____  ___              _    _
| _ \/ _ \ \| _ \_  _ _ _  __| |_ | |
|  _/\__ \> >  _/ || | ' \/ _| ' \|_|
|_|  |___/_/|_|  \_,_|_||_\__|_||_(_)
";
        public static string warning = @"
############################################################
#                                                          #
#    PLEASE NOTE: This is an alpha release of PS>Punch.    #
# There are plenty of bugs and not a lot of functionality. # 
#                                                          #
#         For more info view the release notes at          #
#   https://www.github.com/jaredhaight/pspunch/releases    #
#                                                          #
############################################################

";
        public static string moduleLoadError = "There was an error loading this module \nError message:\n\n{0}\n";
        public static string welcomeMessage = "Welcome to PS>Punch! This is version {0}. \n{1}\nThis is running in .NET v{2}\n";
    }
}
