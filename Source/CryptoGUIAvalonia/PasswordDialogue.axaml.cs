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
using CryptoTranslation;

namespace CryptoGUIAvalonia
{
    public class PasswordDialogue : Window
    {
        private Dict Dictionary;
        private string _inputHash;
        public bool Valid = false;
        public string OutputPw = "";
        private Label lbl_incorrectpassword;
        private Label lbl_key;
        private Label lbl_password;
        private TextBox txt_pw;

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
            lbl_key = this.Get<Label>("lbl_key");
            lbl_password = this.Get<Label>("lbl_password");

            lbl_incorrectpassword = this.Get<Label>("lbl_incorrectpassword");
            txt_pw.KeyDown += Txt_pwOnKeyDown;
            txt_pw.GotFocus += Txt_pw_GotFocus;
            this.Closing += PasswordDialogue_Closing;
            InitializeTranslation();
        }

        private void InitializeTranslation()
        {
            var language = System.Globalization.CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName;
            var engine = new TranslationEngine();
            Dictionary = engine.InitializeLanguage(TranslationEngine.Languages.Contains(language) ? language : "eng");
            lbl_incorrectpassword.Content = Dictionary.PasswordDialogue_IncorrectPassword;
            lbl_password.Content = Dictionary.PasswordDialogue_PasswordLabel;
            var keyReader = new StreamReader(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "config.ini");
            lbl_key.Content = $"{Dictionary.PasswordDialogue_Key} {Path.GetFileName(keyReader.ReadLine())}";
            keyReader.Close();
            this.Title = Dictionary.PasswordDialogue_Title;
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