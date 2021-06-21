using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryptoAPI.ORM;
using CryptoGUIAvalonia.GUI;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;
using CryptoTranslation;
using Ionic.Zip;
using Ionic.Zlib;

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
        private string keyPath = "";
        private TextBox txt_pw;
   
        private bool isResizing = false;
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
            if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "config.ini"))
            {
                var keyReader = new StreamReader(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "config.ini");
                keyPath = keyReader.ReadLine();
                keyReader.Close();
                if(File.Exists(keyPath))
                    lbl_key.Content = $"{Dictionary.PasswordDialogue_Key} {Path.GetFileName(keyPath)}";
                else
                {
                    lbl_key.Content = Dictionary.PasswordDialogue_NoKeyDetected;
                    lbl_key.Foreground = Brush.Parse("Red");
                }
              
            }
            else
            {
                lbl_key.Content = Dictionary.PasswordDialogue_NoKeyDetected;
                lbl_key.Foreground = Brush.Parse("Red");
                
            }
            this.Title = Dictionary.PasswordDialogue_Title;
        }

     
     

        private async Task ShowDialog()
        {
            var dlg = new OpenFileDialog();
            var filter = new FileDialogFilter
            {
                Name = Dictionary.General_FileDialogFilter,
            };
            filter.Extensions.Add("epak");
            dlg.Filters.Add(filter);
            dlg.Title = Dictionary.General_OpenFileDialogTitle;
            var _dlg = dlg.ShowAsync(this);
            var result = false;
            var fileRes = "";
            foreach (var res in await _dlg)
            {
                if (res.EndsWith(".epak"))
                {
                    fileRes = res;
                    result = true;
                }
            }

            if (result)
            {
                keyPath = fileRes;
                lbl_key.Content =    lbl_key.Content = $"{Dictionary.PasswordDialogue_Key} {Path.GetFileName(fileRes)}";
                lbl_key.Foreground = Brush.Parse("Green");
            }
        }
        private void Txt_pwOnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidateKey(keyPath, txt_pw.Text);
            }
        }
    private void ValidateKey(string path, string password)
        {
            var zipFailed = false;
            using (ZipFile zip = ZipFile.Read(path))
            {
          
                zip.CompressionLevel = CompressionLevel.None;
                try
                {
                    zip.ExtractAll(Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
                catch
                {
                    zipFailed = true;
                    lbl_key.Content = Dictionary.PasswordDialogue_ValidationFailed;
                    lbl_key.Foreground = Brushes.Red;
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval");
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey");
                }
            }

            if (!zipFailed)
            {
                if (File.ReadAllLines(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval")[0] ==
                    Cryptography.Encryption.HashPassword(password))
                {
                    lbl_key.Content = Dictionary.PasswordDialogue_ValidationSuccess;
                    lbl_key.Foreground = Brushes.Red;
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval");
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey");
                    OutputPw = txt_pw.Text;
                    Valid = true;
                    Close();
                   
                }
                else
                {
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval");
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey");
                    lbl_incorrectpassword.IsVisible = true;
                    txt_pw.Text = "";
                    lbl_incorrectpassword.Focus();
                }
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
        ValidateKey(keyPath, txt_pw.Text);
        }

   

        private void Btn_changekey_OnClick(object? sender, RoutedEventArgs e)
        {
            ShowDialog();
        }
    }
}