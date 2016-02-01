using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSPunch
{
    class Strings
    {
        public static string version = "0.2.3.1-beta";
        public static string windowTitle = "PSPunch!!";
        public static List<string> pspLogos = new List<string>() {
@"
 ___  _____  ___              _    _
| _ \/ _ \ \| _ \_  _ _ _  __| |_ | |
|  _/\__ \> >  _/ || | ' \/ _| ' \|_|
|_|  |___/_/|_|  \_,_|_||_\__|_||_(_)
",
@"
 (   (      (                        
 )\ ))\ )   )\ )                  )  
(()/(()/(  (()/(  (            ( /(  
 /(_)/(_))  /(_))))\  (     (  )\()) 
(_))(_))__ (_)) /((_) )\ )  )\((_)\  
| _ / __\ \| _ (_))( _(_/( ((_| |(_) 
|  _\__ \> |  _| || | ' \)/ _|| ' \  
|_| |___/_/|_|  \_,_|_||_|\__||_||_| 
                                     
",
@"
           __                         __ 
 _____ ____\ \  _____             _  |  |
|  _  |   __\ \|  _  |_ _ ___ ___| |_|  |
|   __|__   |> |   __| | |   |  _|   |__|
|__|  |_____/ /|__|  |___|_|_|___|_|_|__|
           /_/                           

",
@"
######   #####  #    ######                              ### 
#     # #     #  #   #     # #    # #    #  ####  #    # ### 
#     # #         #  #     # #    # ##   # #    # #    # ### 
######   #####     # ######  #    # # #  # #      ######  #  
#             #   #  #       #    # #  # # #      #    #     
#       #     #  #   #       #    # #   ## #    # #    # ### 
#        #####  #    #        ####  #    #  ####  #    # ### 
",
@"
   _ \    ___| \ \    _ \                       |      | 
  |   | \___ \  \ \  |   |  |   |  __ \    __|  __ \   | 
  ___/        |   /  ___/   |   |  |   |  (     | | | _| 
 _|     _____/  _/  _|     \__,_| _|  _| \___| _| |_| _) 
                                                         
"
        };
        public static string warning = @"
 ############################################################
 #                                                          #
 #     PLEASE NOTE: This is a beta release of PS>Punch      #
 #   Things might be buggy. If you find something that's    #
 #             broken please submit an issue at             #
 #      https://github.com/jaredhaight/pspunch/issues       #
 #        or even better, submit a pull request! :-D        #
 #                                                          #
 #         For more info view the release notes at          #
 #    https://www.github.com/jaredhaight/pspunch/releases   #
 #                                                          #
 ############################################################

";
        public static string moduleLoadError = "There was an error loading this module \nError message:\n\n{0}\n";
        public static string welcomeMessage = "Welcome to PS>Punch! This is version {0}. \n{1}\nFor help getting started, run 'get-attack'\n";
    }
}
