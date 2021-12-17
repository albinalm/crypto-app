#pragma warning disable SYSLIB0023
#pragma warning disable SYSLIB0022
#pragma warning disable CS0618


using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace varbyte.encryption.ORM;

public static class Cryptography
{
    private static byte[] Key { get; set; }
    private static byte[] InitializationVector { get; set; }

    /// <summary>
    ///     <para>Return a random byte value to save when reading the Key property</para>
    ///     <para>Also adapts returned bytes and assigns them to the Key property</para>
    ///     /**/
    /// </summary>
    public static byte[] GenerateEncryptionKey(string password)
    {
        var keyBytes = new byte[2048 * 1024]; // convert kb to byte
        var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(keyBytes);
        ReadEncryptionKey(password, keyBytes);
        return keyBytes;
    }

    /// <summary>
    ///     <para>Input your password in raw format, and also the bytes from when you generated your Key</para>
    ///     <para>This method will set your Key property to it's value</para>
    /// </summary>
    public static void ReadEncryptionKey(string password, byte[] keyBytes)
    {
        var key = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(password), keyBytes, 1000);
        Key = key.GetBytes(256 / 8);
        InitializationVector = key.GetBytes(128 / 8);
    }

    ///<para>Static Encryption-class containing methods to handle encryption</para>
    public static class Encryption
    {
        public static void EncryptFile(string inputFile, string outputFile, long chunkSize)
        {
            var fsOutput = File.OpenWrite(outputFile);
            var fsInput = File.OpenRead(inputFile);
            var symmetricKey = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Key = Key,
                IV = InitializationVector,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.ANSIX923
            };
            var cryptoStream = new CryptoStream(fsOutput, symmetricKey.CreateEncryptor(), CryptoStreamMode.Write);
            for (long i = 0; i < fsInput.Length; i += chunkSize)
            {
                var chunkData = new byte[chunkSize];
                var bytesRead = 0;
                while ((bytesRead = fsInput.Read(chunkData, 0, (int) chunkSize)) > 0)
                {
                    if (bytesRead != chunkSize)
                        for (var x = bytesRead - 1; x < chunkSize; x++)
                            chunkData[x] = 0;
                    cryptoStream.Write(chunkData, 0, (int) chunkSize);
                }
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
            var bytes = sha256Hash.ComputeHash(
                Encoding.UTF8.GetBytes(input + Encoding.ASCII.GetBytes(Environment.UserName)));
            var builder = new StringBuilder();
            foreach (var t in bytes)
                builder.Append(t.ToString("x2"));
            return builder.ToString();
        }
    }

    ///<para>Static Decryption-class containing methods to handle encryption</para>
    public static class Decryption
    {
        public static void DecryptFile(string inputFile, string outputFile, long chunkSize)
        {
            var fsInput = File.OpenRead(inputFile);
            var fsOutput = File.OpenWrite(outputFile);
            var symmetricKey = new RijndaelManaged

            {
                KeySize = 256,
                BlockSize = 128,
                Key = Key,
                IV = InitializationVector,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.ANSIX923
            };
            var cryptoStream = new CryptoStream(fsOutput, symmetricKey.CreateDecryptor(), CryptoStreamMode.Write);
            for (long i = 0; i < fsInput.Length; i += chunkSize)
            {
                var chunkData = new byte[chunkSize];
                var bytesRead = 0;
                while ((bytesRead = fsInput.Read(chunkData, 0, (int) chunkSize)) > 0)
                    cryptoStream.Write(chunkData, 0, bytesRead);
            }

            cryptoStream.Close();
            fsInput.Close();
            fsInput.Dispose();
            cryptoStream.Dispose();
        }
    }
}