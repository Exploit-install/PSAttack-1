using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using PSPunch.PSPunchShell;
using PSPunch.PSPunchProcessing;

namespace PSPunch.Utils
{
    class PSPUtils
    {
        public static void ImportModules(PunchState punchState, Stream moduleStream)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader keyReader = new StreamReader(assembly.GetManifestResourceStream("PSPunch.Modules.key.txt"));
            string key = keyReader.ReadToEnd();
            try
            {
                MemoryStream decMem = CryptoUtils.DecryptFile(moduleStream);
                punchState.cmd = Encoding.Unicode.GetString(decMem.ToArray());
                Processing.PSExec(punchState);
            }
            catch (Exception e)
            {
                ConsoleColor origColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Strings.moduleLoadError, e.Message);
                Console.ForegroundColor = origColor;
            }
        }
    }
}
