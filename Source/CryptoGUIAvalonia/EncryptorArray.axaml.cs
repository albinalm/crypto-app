using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;

namespace CryptoGUIAvalonia
{
    public class EncryptorArray : Window
    {
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public EncryptorArray()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var logoImage = this.Get<Image>("img_icon");
            //logoImage.Source = "/Resources/logo02.png";
            logoImage.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/logo02.png");
        }
    }
}