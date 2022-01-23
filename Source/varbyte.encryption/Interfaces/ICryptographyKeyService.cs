using varbyte.encryption.Models;

namespace varbyte.encryption.Interfaces;

public interface ICryptographyKeyService
{
    public CryptographyKey GenerateEncryptionKey(string password);
    public void WriteCryptographyKey(CryptographyKey key, string destination);
    public CryptographyKey ReadKey(string keyPath, string password);

}