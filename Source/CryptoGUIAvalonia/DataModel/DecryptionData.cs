using System.Collections.Generic;

namespace CryptoGUI.DataModel
{
    public static class DecryptionData
    {
        static DecryptionData()
        {
            Sources = new List<string>();
        }

        public static string SourceFileName { get; set; }
        public static string DestinationFileName { get; set; }
        public static List<string> Sources { get; set; }
        public static List<string> Destinations { get; set; }
    }
}