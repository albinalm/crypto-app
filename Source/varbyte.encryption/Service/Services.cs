using varbyte.encryption.Interfaces;
using varbyte.encryption.ORM;

namespace varbyte.encryption.Service;

public class Services
{
    public readonly ICryptographyService CryptographyService;
    public readonly ICryptographyKeyService CryptographyKeyService;
    public Services()
    {
        CryptographyService = new CryptographyService();
        CryptographyKeyService = new CryptographyKeyService();
    }
}