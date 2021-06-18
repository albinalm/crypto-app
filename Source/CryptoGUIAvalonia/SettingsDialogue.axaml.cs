using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using CryptoAPI.ORM;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;

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