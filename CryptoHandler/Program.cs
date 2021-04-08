using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CryptoHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            
            ProcessStartInfo pinf = new ProcessStartInfo(@"E:\Skolarbete\Rider Projekt\crypto-app\Source\CryptoGUI\bin\Debug\net5.0-windows\CryptoGUI.exe");
            for (int i = 0; i <= args.Length - 1; i++)
            {
                pinf.ArgumentList.Add(args[i]);
            }
            Process.Start(pinf);
        }
     
    }
}
