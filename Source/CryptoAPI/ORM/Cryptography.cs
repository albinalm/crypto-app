using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace CryptoAPI.ORM
{
    public static class Cryptography
    {
     
        public static byte[] Key { get; private set; }
        private static byte[] IV { get; set; }
       
       
        ///<summary>
        ///<para>Return a random byte value to save when reading the Key property</para>
        ///<para>Also adapts returned bytes and assigns them to the Key property</para>/**/
        /// </summary>
        public static byte[] GenerateEncryptionKey(string password)
        {
            var rnd = new Random();
            var b = new byte[2048 * 1024]; // convert kb to byte
            rnd.NextBytes(b);
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(b);
            var key = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(password), b, 1000);
            Key = key.GetBytes(256 / 8);
            IV = key.GetBytes(128 / 8);
            return b;
        }
        
        
        ///<summary>
        ///<para>Input your password in raw format, and also the bytes from when you generated your Key</para>
        ///<para>This method will set your Key property to it's value</para>
        ///</summary>
        public static void ReadEncryptionKey(string password, byte[] keyBytes)
        {
            var key = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(password), keyBytes, 1000);
            Key = key.GetBytes(256 / 8);
            IV = key.GetBytes(128 / 8);
         
        }
        ///<para>Static Encryption-class containing methods to handle encryption</para>
        public static class Encryption
        {
            public static void EncryptFile(string inputFile, string outputFile, long chunkSize)
            {
                FileStream fsOutput = File.OpenWrite(outputFile);
                FileStream fsInput = File.OpenRead(inputFile);
                RijndaelManaged symmetricKey = new RijndaelManaged()
                {
                    KeySize = 256,
                    BlockSize = 128,
                    Key = Key,
                    IV = IV,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.Zeros

                };
                using var cryptoStream = new CryptoStream(fsOutput, symmetricKey.CreateEncryptor(), CryptoStreamMode.Write);

                for (long i = 0; i < fsInput.Length; i += chunkSize)
                {
                    byte[] chunkData = new byte[chunkSize];
                    fsInput.Read(chunkData, 0, (int)chunkSize);
                    cryptoStream.Write(chunkData, 0, chunkData.Length);
                }
                cryptoStream.FlushFinalBlock();
                cryptoStream.Close();
                fsInput.Close();
                fsInput.Dispose();
                cryptoStream.Dispose();
            }
           

            public static string HashPassword(string input)
            {
                using var sha256Hash = SHA256.Create();
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input + Encoding.ASCII.GetBytes(Environment.UserName)));
                var builder = new StringBuilder();
                foreach (var t in bytes)
                    builder.Append(t.ToString("x2"));
                return builder.ToString();
            }
        }
    
        ///<para>Static Decryption-class containing methods to handle encryption</para>
        public static class Decryption
        { 
            public static void DecryptFile_deprecated(string inputFile, string outputFile)
            {
                var inputBytes = File.ReadAllBytes(inputFile);
                var ms = new MemoryStream();
                var rm = new RijndaelManaged
                {
                    KeySize = 256, 
                    BlockSize = 128,
                    Key = Key,
                    IV = IV,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.Zeros
                };
                using var stream = new CryptoStream(ms, rm.CreateDecryptor(), CryptoStreamMode.Write);
                stream.Write(inputBytes);
                stream.Close();
                File.WriteAllBytes(outputFile, ms.ToArray());
            }
            public static void DecryptFile(string inputFile, string outputFile, long chunkSize)
            {
                FileStream fsOutput = File.OpenWrite(outputFile);
                FileStream fsInput = File.OpenRead(inputFile);
                RijndaelManaged symmetricKey = new RijndaelManaged()
                {
                    KeySize = 256,
                    BlockSize = 128,
                    Key = Key,
                    IV = IV,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.Zeros
                };
                using var cryptoStream = new CryptoStream(fsOutput, symmetricKey.CreateDecryptor(), CryptoStreamMode.Write);

                for (long i = 0; i < fsInput.Length; i += chunkSize)
                {
                    byte[] chunkData = new byte[chunkSize];
                    int bytesRead = 0;
                    while((bytesRead = fsInput.Read(chunkData, 0, (int)chunkSize)) > 0)
                    {
                        cryptoStream.Write(chunkData, 0, bytesRead);
                    }
                }
                cryptoStream.Close();
                fsInput.Close();
                fsInput.Dispose();
                cryptoStream.Dispose();
            }
        }
    }
}