using System;
using System.Diagnostics;
using System.IO;
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
        private Label lbl_incorrectpassword { get; set; }
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
            logoImage.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/logo01.png");
            Icon = new WindowIcon(new Bitmap(Environment.CurrentDirectory + "/Resources/icon.png"));
            var keyReader = new StreamReader(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "config.ini");
            this.Get<Label>("lbl_key").Content = $"Key: {Path.GetFileName(keyReader.ReadLine())}";
            lbl_incorrectpassword = this.Get<Label>("lbl_incorrectpassword");
            txt_pw.KeyDown += Txt_pwOnKeyDown;
            keyReader.Close();
            txt_pw.GotFocus += Txt_pw_GotFocus;
            this.Closing += PasswordDialogue_Closing;
        }

        private void Txt_pwOnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Validate();
            }
        }

        private void PasswordDialogue_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Valid)
            {
                Environment.Exit(0);
            }
        }

        private void Txt_pw_GotFocus(object? sender, GotFocusEventArgs e)
        {
            lbl_incorrectpassword.IsVisible = false;
        }

        private void btn_confirm_Click(object sender, RoutedEventArgs e)
        {
         Validate();
        }

        private void Validate()
        {
            if (Cryptography.Encryption.HashPassword(txt_pw.Text) == _inputHash)
            {
                OutputPw = txt_pw.Text;
                Valid = true;
                Close();
            }
            else
            {
                lbl_incorrectpassword.IsVisible = true;
                txt_pw.Text = "";
                lbl_incorrectpassword.Focus();
            }
        }
    }
}