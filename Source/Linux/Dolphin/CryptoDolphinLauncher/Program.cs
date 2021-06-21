using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace CryptoDolphinLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(new Random().Next(0, 500)); //Random launch time to avoid deadlock
            Environment.CurrentDirectory = AppContext.BaseDirectory.EndsWith(Path.DirectorySeparatorChar) ? AppContext.BaseDirectory.Remove(AppContext.BaseDirectory.Length - 1, 1) : AppContext.BaseDirectory;
            var writer = new StreamWriter(Environment.CurrentDirectory + "/log.txt", true);
            if (Process
                .GetProcessesByName("CryptoDolphinLauncher")
                .Length > 1)
            {
            
                Process.GetCurrentProcess().Kill();
            }
            
            var mode = args.SingleOrDefault(s => s.StartsWith("CryptoApp_CommandArgs_"));
      
         
            try
            {
                writer.WriteLine("Starting");
                writer.Flush();
                var exists = false;
                while (!exists)
                {
                    writer.WriteLine("Iterating");
                    writer.Flush();
                    if (Process.GetProcessesByName("CryptoDolphinFileWriter").Length < 1)
                    {
                        exists = true;
                    }
                }
                writer.WriteLine("Sleeping");
                writer.Flush();
           
                var path = Directory.GetParent(Environment.CurrentDirectory).FullName;
                path += "/CryptoGUIAvalonia";
                
                writer.WriteLine($"Path set to: {path}");
                writer.Flush();
                var appargs = SetArgs();
                writer.WriteLine($"Args is: {appargs}");
                writer.Flush();
                writer.WriteLine("Starting process");
                writer.Flush();
                Process.Start(path, appargs);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                writer.Write(ex.ToString());
                writer.Flush();
                writer.Close();
            }
           
        }

        private static string SetArgs()
        {
            //var process = new ProcessStartInfo(filename);
            var args = "";
            foreach (var finf in new DirectoryInfo(Environment.CurrentDirectory + "/writer").GetFiles())
            {
                if (finf.Extension == ".temp_sourceListArray")
                {
                    foreach (var line in File.ReadAllLines(finf.FullName))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            if (args == "")
                            {
                                args += "\"" + line + "\"";
                            }
                            else
                            {
                                if(!args.Contains(line))
                                    args += " " + "\"" + line + "\"";
                            }
                        }
                    }
                    File.Delete(finf.FullName);
                }
            }
            return args;
        }
    }
}