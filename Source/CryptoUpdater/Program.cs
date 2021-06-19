using System;
using System.Diagnostics;
using Ionic.Zip;

namespace CryptoUpdater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (ZipFile zip = ZipFile.Read(Environment.CurrentDirectory + @"/update.zip"))
            {
                zip.ExtractAll(Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently);
            }
            if (Environment.OSVersion.ToString().Contains("Windows"))
            {
                Process.Start(Environment.CurrentDirectory + @"/CryptoGUIAvalonia.exe");
                Environment.Exit(0);
            }
            else if (Environment.OSVersion.ToString().Contains("Unix"))
            {
                Process.Start(Environment.CurrentDirectory + @"/CryptoGUIAvalonia.exe");
                Environment.Exit(0);
            }
        }
    }
}