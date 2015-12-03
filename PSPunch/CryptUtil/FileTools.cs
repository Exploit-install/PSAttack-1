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

        public static MemoryStream DecryptFile(Stream inputStream, string key)
        {
            byte[] keyBytes = Encoding.Unicode.GetBytes(key);

            Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(key, keyBytes);

            RijndaelManaged rijndaelCSP = new RijndaelManaged();
            rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
            rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);
            ICryptoTransform decryptor = rijndaelCSP.CreateDecryptor();

            CryptoStream decryptStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read);
            byte[] inputFileData = new byte[(int)inputStream.Length];
            string contents = new StreamReader(decryptStream).ReadToEnd();
            byte[] unicodes = Encoding.Unicode.GetBytes(contents);

            MemoryStream outputMemoryStream = new MemoryStream(unicodes);
            rijndaelCSP.Clear();

            decryptStream.Close();
            inputStream.Close();
            return outputMemoryStream;
        }
    }
}
