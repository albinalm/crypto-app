using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CryptoAPI.ORM;

namespace CryptoTerminal
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Generate new key");
            File.WriteAllBytes(@"C:\users\albin\desktop\key.key",Cryptography.GenerateEncryptionKey(Cryptography.Encryption.HashPassword("ost123")));
            Console.WriteLine("Password hash: " + Cryptography.Encryption.HashPassword("ost123"));
            Console.WriteLine("Key generated. Press any key to continue with encryption");
            Console.ReadKey();
            Console.WriteLine("Init. Key & Iv");
            Cryptography.ReadEncryptionKey(Cryptography.Encryption.HashPassword("ost123"), File.ReadAllBytes(@"C:\users\albin\desktop\key.key"));
            Console.WriteLine("Finished");
            Console.WriteLine("Writing File");
            Cryptography.Encryption.EncryptFile(@"C:\users\albin\desktop\fileToEncrypt.exe", @"C:\users\albin\desktop\encrypted.exe");
            Console.WriteLine("Finished encrypting. Press any key to decrypt");
            Console.ReadKey();
            Cryptography.ReadEncryptionKey(Cryptography.Encryption.HashPassword("ost123"), File.ReadAllBytes(@"C:\users\albin\desktop\key.key"));
            Console.WriteLine("Writing File");
            Cryptography.Decryption.DecryptFile(@"C:\users\albin\desktop\encrypted.exe", @"C:\users\albin\desktop\decrypted.exe");
            Console.WriteLine("Finished decrypting");
        }
        public static void PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.Append(b + ", ");
            }
            sb.Append("}");
            Console.WriteLine(sb.ToString());
        }
    }
}