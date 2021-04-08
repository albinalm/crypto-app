using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoGUI.DataModel
{
    public static class DecryptionData
    {
        public static string SourceFileName { get; set; }
        public static string DestinationFileName { get; set; }
        public static List<string> Sources { get; set; }
        public static List<string> Destinations { get; set; }
    }
}
