using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;

namespace CryptoGUIAvalonia
{
    public class KeyUpdateDialog : Window
    {
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public KeyUpdateDialog()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }
    }
}