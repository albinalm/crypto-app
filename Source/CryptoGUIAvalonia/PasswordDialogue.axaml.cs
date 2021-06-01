using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CryptoAPI.ORM;
using CryptoGUIAvalonia.GUI;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;

namespace CryptoGUIAvalonia
{
    public class PasswordDialogue : Window
    {
        private readonly string _inputHash;
        private readonly DataShareInstance _dsInstance;
        public string OutputPw = "";
        private TextBox txt_pw { get; set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public PasswordDialogue()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public PasswordDialogue(DataShareInstance dsInstance)
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _dsInstance = dsInstance;
            _inputHash = "";
            txt_pw = this.Get<TextBox>("txt_pw");
            var logoImage = this.Get<Image>("img_icon");
            //logoImage.Source = "/Resources/logo02.png";
            logoImage.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/logo02.png");
        }

        private void btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            if (Cryptography.Encryption.HashPassword(txt_pw.Text) == _inputHash)
            {
                OutputPw = txt_pw.Text;

                Close();
            }
            else
            {
                Close();
            }
        }
    }
}