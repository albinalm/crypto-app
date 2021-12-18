namespace varbyte.encryption.Interfaces;

public interface ICryptography
{
    public byte[] GenerateEncryptionKey(string password);
    public void ReadEncryptionKey(string password, byte[] keyBytes);
    public string HashPassword(string input);
    public void EncryptFile(string inputFile, string outputFile, long chunkSize);
    public void DecryptFile(string inputFile, string outputFile, long chunkSize);
}