using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CryptoGUIAvalonia.GUI.Dialogues;

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
                desktop.MainWindow = new Encryptor();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}