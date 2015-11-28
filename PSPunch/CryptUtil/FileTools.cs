using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.Collections.Generic;
namespace PSPunch.CryptUtil
{
    class FileTools
    {
        public static void EncryptFile(string inputFile, string outputFile, string key)
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

        public static MemoryStream DecryptFile(string inputFile, string key)
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
            string contents = new StreamReader(decryptStream).ReadToEnd();
            byte[] unicodes = Encoding.Unicode.GetBytes(contents);

            //FileStream outputFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            //outputFileStream.Write(inputFileData, 0, inputFileData.Length);
            //outputFileStream.Flush();

            MemoryStream outputMemoryStream = new MemoryStream(unicodes);

            rijndaelCSP.Clear();

            decryptStream.Close();
            inputFileStream.Close();
            return outputMemoryStream;
        }
    }
}
