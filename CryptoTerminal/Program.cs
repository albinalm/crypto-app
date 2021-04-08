using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using CryptoAPI.ORM;

namespace CryptoTerminal
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread thread = new Thread(new ThreadStart(tracker));
            thread.IsBackground = true;
           // thread.Start();
            Console.WriteLine("Generate new key");
            File.WriteAllBytes(@"C:\users\albin\desktop\key.key",Cryptography.GenerateEncryptionKey(Cryptography.Encryption.HashPassword("ost123")));
            Console.WriteLine("Password hash: " + Cryptography.Encryption.HashPassword("ost123"));
            Console.WriteLine("Key generated. Press any key to continue with encryption");
            Console.ReadKey();
            Console.WriteLine("Init. Key & Iv");
            Cryptography.ReadEncryptionKey(Cryptography.Encryption.HashPassword("ost123"), File.ReadAllBytes(@"C:\users\albin\desktop\key.key"));
            Console.WriteLine("Finished");
            Console.WriteLine("Writing File Stream");
            Cryptography.Encryption.EncryptFile(@"C:\Users\Albin\Downloads\Downloads.7z", @"C:\users\albin\desktop\encrypted", 1024);
            Console.WriteLine("Finished encrypting. Press any key to decrypt");
            Console.ReadKey();
            Cryptography.ReadEncryptionKey(Cryptography.Encryption.HashPassword("ost123"), File.ReadAllBytes(@"C:\users\albin\desktop\key.key"));
            Console.WriteLine("Writing File");
            Cryptography.Decryption.DecryptFile(@"C:\users\albin\desktop\encrypted", @"C:\users\albin\desktop\decrypted.7z", 1024);
            Console.WriteLine("Finished decrypting");
        }
        static void tracker()
        {
            while (true)
            {
                if (File.Exists(@"C:\users\albin\desktop\encrypted"))
                {
                    FileInfo finf = new FileInfo(@"C:\users\albin\desktop\encrypted");
                    Console.WriteLine(finf.Length);
                }
               
            }
        }
        static void copier()
        {

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