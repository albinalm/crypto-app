using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CryptoTranslation
{
    public class TranslationEngine
    {
        private readonly Dictionary<string, string> Dictionary;

        public TranslationEngine()
        {
            Dictionary = new Dictionary<string, string>();
        }

        public Dict InitializeLanguage(string lang)
        {
            var dict = new Dict();
            var line = "";
            var assembly = Assembly.GetExecutingAssembly();
            // var resourceName = "MyCompany.MyProduct.MyFile.txt";
            using (var stream = assembly.GetManifestResourceStream("CryptoTranslation.Translations." + lang + ".lang"))
            using (var reader = new StreamReader(stream))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    //Don't do anything if line is just blank or doesn't contain double equals
                    if (string.IsNullOrWhiteSpace(line) || !line.Contains("==")) continue;
                    var lineSplit = line.Split("==");
                    Dictionary.Add(lineSplit[0], lineSplit[1]);
                    foreach (var prop in dict.GetType().GetProperties())
                        prop.SetValue(dict, Dictionary.SingleOrDefault(k => k.Key == prop.Name).Value);
                }
            }

            return dict;
        }

        public static class Languages
        {
            public const string swe = "swe";
            public const string eng = "eng";

            public static bool Contains(string input)
            {
                return typeof(Languages).GetFields().Select(f => f.Name.ToLower()).ToList().Contains(input.ToLower());
            }

            public static List<string> GetLanguages()
            {
                return typeof(Languages).GetFields().Select(f => f.Name).ToList();
            }

            public static string Parse(string input)
            {
                return typeof(Languages).GetFields().Select(f => f.Name).SingleOrDefault(l => string.Equals(l, input, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}