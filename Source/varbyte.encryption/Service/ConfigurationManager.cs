using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using varbyte.encryption.Interfaces;
using varbyte.encryption.Models;

namespace varbyte.encryption.Service
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly string _configPath;
        public Globals Globals { get; }
        private readonly Dictionary<string, object> Dictionary;
        public ConfigurationManager(string configPath)
        {
            _configPath = configPath;
            Dictionary = new Dictionary<string, object>();
            Globals = InitializeConfiguration();
         
        }
        public Globals InitializeConfiguration()
        {
            var globals = new Globals();
            if(File.Exists(_configPath))
            {
                var line = "";
                using (var reader = new StreamReader(_configPath))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line) || !line.Contains("==")) continue;
                        var lineSplit = line.Split("==");


                        if (lineSplit[0] == "int64")
                            Dictionary.Add(lineSplit[1], long.Parse(lineSplit[2]));
                        if (lineSplit[0] == "string")
                            Dictionary.Add(lineSplit[1], lineSplit[2]);


                        foreach (var prop in globals.GetType().GetProperties())
                            prop.SetValue(globals, Dictionary.SingleOrDefault(k => k.Key == prop.Name).Value);
                    }
                }
                return globals;
            }
            return globals.GenerateDefault(_configPath);
           
        }
        public void SetPrimaryKeyPath(string keyPath)
        {

            var contents = File.ReadAllLines(_configPath);
            var found = false;
            for (var i = 0; i < contents.Length; i++)
            {
                if (contents[i].StartsWith("string==PrimaryKeyPath"))
                {
                    found = true;
                    contents[i] = "string==PrimaryKeyPath==" + keyPath;
                    File.WriteAllLines(_configPath, contents);
                    Globals.PrimaryKeyPath = keyPath;
                    break;
                }
            }

            if (!found)
            {
                var updatedContents = contents.ToList();
                updatedContents.Add("string==PrimaryKeyPath==" + keyPath);
                File.WriteAllLines(_configPath, updatedContents);
                Globals.PrimaryKeyPath = keyPath;
            }
        }

    }
}
