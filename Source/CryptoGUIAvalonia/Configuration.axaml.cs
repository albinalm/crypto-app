using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;

namespace CryptoGUIAvalonia
{
    public class Configuration : Window
    {
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public Configuration()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var lblhash = this.Get<Label>("lbl_hash");
            lblhash.FontSize = 12;
        }

        private void txt_keyData_OnPointerEnter(object? sender, PointerEventArgs e)
        {
        }
    }
}