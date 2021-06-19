using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CryptoDolphinFileWriter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
          
            /*
            var file = new DirectoryInfo(Environment.CurrentDirectory).GetFiles()
                .FirstOrDefault(fi => fi.Extension == ".temp_sourceListArray");
            var fileName = "";
            fileName = file != null ? file.FullName : GenerateFileName();
            */
            var fileWriter = new StreamWriter(GenerateFileName(), true);
            foreach (var arg in args)
            {
                fileWriter.WriteLine(arg);
            }
            fileWriter.Flush();
            fileWriter.Close();
           
            
            if(AppContext.BaseDirectory.EndsWith(Path.DirectorySeparatorChar))
                    Process.Start(Directory.GetParent(AppContext.BaseDirectory
                        .Remove(AppContext.BaseDirectory.Length - 1, 1))
                        ?.FullName  + "/CryptoDolphinLauncher");
            else
                Process.Start(Directory.GetParent(AppContext.BaseDirectory)?.FullName + "/CryptoDolphinLauncher");
            
            Environment.Exit(0);
        }
        private static string GenerateFileName()
        {
            var fileName = "";
            var found = false;
            while (!found)
            {
                var lowercaseAlphabet = "abcdefghijklmnoprstuvwxyz";
                var uppercaseAlphabet = lowercaseAlphabet.ToUpper();
                fileName = $"temp__{lowercaseAlphabet[GetRandomIndex(0, lowercaseAlphabet.Length - 1)]}" +
                           $"{lowercaseAlphabet[GetRandomIndex(0, lowercaseAlphabet.Length - 1)]}" +
                           $"{lowercaseAlphabet[GetRandomIndex(0, lowercaseAlphabet.Length - 1)]}" +
                           $"{lowercaseAlphabet[GetRandomIndex(0, lowercaseAlphabet.Length - 1)]}" +
                           $"{uppercaseAlphabet[GetRandomIndex(0, lowercaseAlphabet.Length - 1)]}" +
                           $"{uppercaseAlphabet[GetRandomIndex(0, lowercaseAlphabet.Length - 1)]}" +
                           $"{uppercaseAlphabet[GetRandomIndex(0, lowercaseAlphabet.Length - 1)]}" +
                           $"{uppercaseAlphabet[GetRandomIndex(0, lowercaseAlphabet.Length - 1)]}" +
                           $"{new Random().Next(0, 999)}" +
                           $"{new Random().Next(0, 999)}" +
                           $"{new Random().Next(0, 999)}" +
                           $"{new Random().Next(0, 999)}" +
                           $"{new Random().Next(0, 999)}.temp_sourceListArray";
              
                if (!File.Exists($"{AppContext.BaseDirectory}/{fileName}"))
                    found = true;
             
            }
            return $"{AppContext.BaseDirectory}/{fileName}";
        }

        private static int GetRandomIndex(int start, int end)
        {
            var iterations = new Random().Next(33, 137);
            var iteration = 0;
            var result = 0;
            while (iteration < iterations)
            {
                result = new Random().Next(start, end);
                iteration++;
            }
            return result;
        }
    }
}