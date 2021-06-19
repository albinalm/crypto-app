using System;
using System.Diagnostics;
using System.Reflection;
using Ionic.Zip;
using System.IO;
using System.Threading;

namespace CryptoUpdater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Thread.Sleep(1000);
            using (ZipFile zip = ZipFile.Read(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/update.zip"))
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
                Process.Start(Environment.CurrentDirectory + @"/CryptoGUIAvalonia");
                Environment.Exit(0);
            }
        }
    }
}