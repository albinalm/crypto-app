using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CryptoInstaller
{
    public class MainWindow : Window
    {
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public MainWindow()
        {

            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif

        }

    }
}