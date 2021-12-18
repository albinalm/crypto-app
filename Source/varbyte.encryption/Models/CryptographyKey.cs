using System;
using System.Security.Cryptography;
using System.Text;

namespace varbyte.encryption.Models;

public class CryptographyKey
{
    public readonly byte[] Key;
    public readonly byte[] InitializationVector;
    public readonly string PasswordHash;

    public CryptographyKey(string passwordHash, byte[] initializationVector, byte[] key)
    {
        PasswordHash = passwordHash;
        InitializationVector = initializationVector;
        Key = key;
    }
 
}