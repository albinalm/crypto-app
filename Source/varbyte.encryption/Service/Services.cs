using System.IO;
using varbyte.encryption.Interfaces;
using varbyte.encryption.ORM;

namespace varbyte.encryption.Service;

public class Services
{
    public readonly ICryptographyService CryptographyService;
    public readonly ICryptographyKeyService CryptographyKeyService;
    public readonly IConfigurationManager ConfigurationManager;
    public Services()
    {
        CryptographyService = new CryptographyService();
        ConfigurationManager = new ConfigurationManager(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\appsettings.ini");
        CryptographyKeyService = new CryptographyKeyService();
    }
}