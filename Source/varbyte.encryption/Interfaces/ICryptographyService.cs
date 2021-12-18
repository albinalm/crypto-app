using varbyte.encryption.Models;

namespace varbyte.encryption.Interfaces;

public interface ICryptographyService
{
    public void DecryptFile(string inputFile, string outputFile, CryptographyKey key, long chunkSize);
    public void EncryptFile(string inputFile, string outputFile, CryptographyKey key, long chunkSize);
}