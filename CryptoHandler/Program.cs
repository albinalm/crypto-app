using System.Diagnostics;

namespace CryptoHandler
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var pinf = new ProcessStartInfo(
                @"E:\Skolarbete\Rider Projekt\crypto-app\Source\CryptoGUI\bin\Debug\net5.0-windows\CryptoGUI.exe");
            for (var i = 0; i <= args.Length - 1; i++) pinf.ArgumentList.Add(args[i]);
            Process.Start(pinf);
        }
    }
}