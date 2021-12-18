using System.IO;
using System.Security.Cryptography;
using varbyte.encryption.Interfaces;
using varbyte.encryption.Models;

namespace varbyte.encryption.ORM;
#pragma warning disable SYSLIB0022
#pragma warning disable CS0618
public class CryptographyService : ICryptographyService
{
    public void DecryptFile(string inputFile, string outputFile, CryptographyKey key, long chunkSize)
    {
        var fsInput = File.OpenRead(inputFile);
        var fsOutput = File.OpenWrite(outputFile);

        var symmetricKey = new RijndaelManaged
        {
            KeySize = 256,
            BlockSize = 128,
            Key = key.Key,
            IV = key.InitializationVector,
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
    public void EncryptFile(string inputFile, string outputFile, CryptographyKey key, long chunkSize)
    {
        var fsOutput = File.OpenWrite(outputFile);
        var fsInput = File.OpenRead(inputFile);
        var symmetricKey = new RijndaelManaged
        {
            KeySize = 256,
            BlockSize = 128,
            Key = key.Key,
            IV = key.InitializationVector,
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
}