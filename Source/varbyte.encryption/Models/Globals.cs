using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varbyte.encryption.Models
{
    public class Globals
    {
        public long ChunkSize { get; set; }
        public string PrimaryKeyPath { get; set;
        }
        public Globals GenerateDefault(string configPath)
        {
            var globals = new Globals();

            #region Defaults
            globals.ChunkSize = 1024;

            #endregion Defaults
           
            using var writer = new StreamWriter(configPath);
            foreach (var prop in globals.GetType().GetProperties())
            {
                if(prop.GetValue(globals) != null)
                {
                    writer.WriteLine(prop.PropertyType.Name.ToLower() + "==" + prop.Name + "==" + prop.GetValue(globals).ToString());
                }
            }
           
            writer.Flush();
            writer.Close();

            return globals; 
        }
    }

}
