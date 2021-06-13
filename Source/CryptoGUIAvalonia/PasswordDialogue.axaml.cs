using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private string _inputHash;
        public bool Valid = false;
        public string OutputPw = "";
        private TextBox txt_pw { get; set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public PasswordDialogue()
        {
        }

        public PasswordDialogue(string input)
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            txt_pw = this.Get<TextBox>("txt_pw");
            _inputHash = input;
            var logoImage = this.Get<Image>("img_icon");
            //logoImage.Source = "/Resources/logo02.png";
            logoImage.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/logo02.png");
            Icon = new WindowIcon(new Bitmap(Environment.CurrentDirectory + "/Resources/icon.png"));
        }

        private void btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            if (Cryptography.Encryption.HashPassword(txt_pw.Text) == _inputHash)
            {
                OutputPw = txt_pw.Text;
                Valid = true;
                Close();
            }
            else
            {
                Valid = false;
                Close();
            }
        }
    }
}