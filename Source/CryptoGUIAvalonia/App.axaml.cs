using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CryptoGUIAvalonia.GUI.Dialogues;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;

namespace CryptoGUIAvalonia
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
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}