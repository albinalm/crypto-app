using System;
using System.Diagnostics;
using Ionic.Zip;
using System.IO;
using System.Threading;
using Mono.Unix;

namespace CryptoUpdater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var errorWriter = new StreamWriter(AppContext.BaseDirectory + @"/debug.log", true);
            errorWriter.WriteLine("");
            errorWriter.WriteLine(DateTime.Now.ToString("g"));
            errorWriter.WriteLine("Instantiated writer...");
            
            try
            {
                Thread.Sleep(1000);
                using (ZipFile zip = ZipFile.Read(System.AppContext.BaseDirectory + @"/update.zip"))
                {
                    errorWriter.WriteLine("Extracting zip...");
                    errorWriter.Flush();
                    zip.ExtractAll(AppContext.BaseDirectory, ExtractExistingFileAction.OverwriteSilently);
                }

                errorWriter.WriteLine("Zip extracted");
                errorWriter.WriteLine("Checking os...");
                errorWriter.Flush();
                if (Environment.OSVersion.ToString().Contains("Windows"))
                {
                    Process.Start(AppContext.BaseDirectory + @"/CryptoGUIAvalonia.exe");
                    Environment.Exit(0);
                }
                else if (Environment.OSVersion.ToString().Contains("Unix"))
                {
                    errorWriter.WriteLine("OS is linux");
                    errorWriter.WriteLine("Setting file permissions");
                  //  Exec("chmod 644 " + AppContext.BaseDirectory + @"/CryptoGUIAvalonia");
                  Exec(AppContext.BaseDirectory + @"/CryptoGUIAvalonia");
                  Exec(AppContext.BaseDirectory + @"/contextmenubinaries/CryptoDolphinLauncher");
                  Exec(AppContext.BaseDirectory + @"/contextmenubinaries/writer/CryptoDolphinFileWriter");
                  errorWriter.WriteLine("Starting Avalonia");
                    Process.Start(AppContext.BaseDirectory + @"/CryptoGUIAvalonia");
                   
                    errorWriter.WriteLine("Exiting bye!");
                    errorWriter.Flush();
                    Environment.Exit(0);
                }
            }
            catch(Exception ex)
            {
                errorWriter.WriteLine("Threw!!");
                errorWriter.WriteLine("");
                errorWriter.WriteLine("");
                errorWriter.WriteLine(ex.ToString());
                errorWriter.Flush();
                errorWriter.Close();
            }
          
        }
        private static void Exec(string cmd)
        {
            var unixFileInfo = new Mono.Unix.UnixFileInfo(cmd);
            // set file permission to 644
            unixFileInfo.FileAccessPermissions =
                FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite
                                               | FileAccessPermissions.GroupRead
                                               | FileAccessPermissions.OtherRead 
                                               | FileAccessPermissions.UserExecute;
        }
    }
}