using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace CryptoInstaller
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                /*
                 *
                 *     desktop.MainWindow = new MessageBox("Insufficent diskspace", "Insufficent disk space",
                    "The disk space has run out\nPlease retry after cleaning up!", MessageBox.MessageBoxButtons.Ok);
                 */
                Environment.CurrentDirectory = AppContext.BaseDirectory.EndsWith(Path.DirectorySeparatorChar) ? AppContext.BaseDirectory.Remove(AppContext.BaseDirectory.Length - 1, 1) : AppContext.BaseDirectory;
            //    desktop.MainWindow = new PasswordDialogue("xD");
              desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}