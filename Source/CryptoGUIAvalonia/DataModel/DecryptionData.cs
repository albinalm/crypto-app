using System.Collections.Generic;

namespace CryptoGUI.DataModel
{
    public static class DecryptionData
    {
        static DecryptionData()
        {
            Sources = new List<string>();
            Destinations = new List<string>();
        }

        public static List<string> Sources { get; set; }
        public static List<string> Destinations { get; set; }
    }
}