using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading;

namespace CryptoInstallerLinuxExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (!Directory.Exists(AppContext.BaseDirectory + @"/install"))
                    Directory.CreateDirectory(AppContext.BaseDirectory + @"/install");

                WriteInstallFile("CryptoInstallerLinux");
                WriteInstallFile("libHarfBuzzSharp.so");
                WriteInstallFile("libMonoPosixHelper.so");
                WriteInstallFile("libSkiaSharp.so");
                Console.WriteLine(
                Bash("chmod u=rwx " + '"' + AppContext.BaseDirectory + @"install/CryptoInstallerLinux" + '"'));
                var proc = new ProcessStartInfo();
                proc.FileName = AppContext.BaseDirectory + @"/install/CryptoInstallerLinux";
                var p = Process.Start(proc);
                p.WaitForExit();
                Thread.Sleep(1000);
                Directory.Delete(AppContext.BaseDirectory + @"/install", true);
            }
            catch (Exception ex)
            {
                File.WriteAllText(AppContext.BaseDirectory + @"/log.log", ex.ToString());
            }
         
        }
        private static void WriteInstallFile(string resourceName)
        {
            using(var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("CryptoInstallerLinuxExtractor.InstallData." + resourceName))
            {
                using(var file = new FileStream(AppContext.BaseDirectory + @"/install/" + resourceName, FileMode.Create, FileAccess.Write))
                {
                    resource?.CopyTo(file);
                } 
            }
        }
        public static string Bash(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result;
        }
    }
}