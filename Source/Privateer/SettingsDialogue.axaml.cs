using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;

namespace CryptoGUIAvalonia
{
    public class SettingsDialogue : Window
    {

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public SettingsDialogue()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Icon = new WindowIcon(new Bitmap(Environment.CurrentDirectory + "/Resources/icon.png"));
         
        }


    }
}