using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using varbyte.encryption.Interfaces;
using varbyte.encryption.Models;
using varbyte.encryption.Models.Exceptions;
using varbyte.encryption.Service;

namespace varbyte.encryption.ORM;
#pragma warning disable CS0618
#pragma warning disable SYSLIB0023
public class CryptographyKeyService : ICryptographyKeyService
{
    private readonly string _baseDir;
    public CryptographyKeyService()
    {
        _baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      
    }
    public CryptographyKey GenerateEncryptionKey(string password)
    {
        var keyBytes = new byte[2048 * 1024]; // convert kb to byte

        var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(keyBytes);
        var key = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(password), keyBytes, 1000);
        return new CryptographyKey(HashPassword(password), key.GetBytes(128 / 8), key.GetBytes(256 / 8));
    }

    public void WriteCryptographyKey(CryptographyKey key, string destination)
    { 
        if (destination == null) throw new InvalidOperationException("Destination parameter cannot be null");
        
     if(File.Exists(destination))
         File.Delete(destination);
  
     var path = _baseDir + @$"\{Path.GetFileNameWithoutExtension(destination)}";
     Directory.CreateDirectory(path);
     File.WriteAllBytes(path + @"\value.key", key.Key);
     File.WriteAllText(path + @"\value.val", key.PasswordHash);
     ZipFile.CreateFromDirectory(path, destination, CompressionLevel.NoCompression, false);
     Directory.Delete(path, true);
    }

    public CryptographyKey ReadKey(string keyPath, string password)
    {
       
        var path = _baseDir + @$"\{Path.GetFileNameWithoutExtension(keyPath)}";
        if(Directory.Exists(path)) Directory.Delete(path, true);
        Directory.CreateDirectory(path);
        var keyBytes = new byte[1];
        var keyHash = "";
        ZipFile.ExtractToDirectory(keyPath, path);
        keyBytes = File.ReadAllBytes(path + @"\value.key");
        keyHash = File.ReadAllText(path + @"\value.val");
        Directory.Delete(path, true);
      
        if (HashPassword(password) != keyHash)  throw new PasswordIncorrectException("The input password is incorrect");
        var keyData = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(password), keyBytes, 1000);
        return new CryptographyKey(HashPassword(password), keyData.GetBytes(128 / 8), keyData.GetBytes(256 / 8));
        
      

    }
    private static string HashPassword(string input)
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