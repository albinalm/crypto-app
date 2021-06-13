using System;
using System.ComponentModel;
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
using Avalonia.Platform;
using Avalonia.Threading;
using CryptoAPI.ORM;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;
using Ionic.Zip;
using Ionic.Zlib;

namespace CryptoGUIAvalonia
{
    
    public class Index : Window
    {
        private Label lbl_validation;
        private Label lbl_details;
        private Label lbl_enterkey;
        private Label lbl_keyfound;
        private Label lbl_keydate;
        private Label lbl_keylocation;
        private TextBox txt_pass;
        private Button btn_newkey;
        private Button btn_loadkey;
        private Button btn_validatekey;
        private string Mode = "";
        private string LoadKey_FileName = "";
        private string ValidateKey_FileName = "";
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public Index()
        {
          
          
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var logoImage = this.Get<Image>("img_icon");
            //logoImage.Source = "/Resources/logo02.png";
            logoImage.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/logo01.png");
            this.Get<TextBox>("txt_pass").GotFocus += txt_pass_gotFocus;
        //    this.Icon = new WindowIcon(new Bitmap(Environment.CurrentDirectory + "/Resources/icon.png"));
            lbl_validation = this.Get<Label>("lbl_validation");
            lbl_details = this.Get<Label>("lbl_details");
            lbl_enterkey = this.Get<Label>("lbl_enterkey");
            lbl_keyfound = this.Get<Label>("lbl_keyfound");
            lbl_keydate = this.Get<Label>("lbl_keydate");
            lbl_keylocation = this.Get<Label>("lbl_keylocation");
            txt_pass = this.Get<TextBox>("txt_pass");
            btn_newkey = this.Get<Button>("btn_newkey");
            btn_loadkey = this.Get<Button>("btn_loadkey");
            btn_validatekey = this.Get<Button>("btn_validatekey");
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "config.ini"))
            {
                var keyReader = new StreamReader(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "config.ini");
                var keyPath = keyReader.ReadLine();
                keyReader.Close();
                lbl_keylocation.Content = $"Expected location: {keyPath}";
                lbl_keydate.Content = "Created: --";
                lbl_keylocation.Foreground = Brushes.Black;
                if (File.Exists(keyPath))
                {
                    lbl_keydate.Content = $"Created: {new FileInfo(keyPath).CreationTime.ToString("g")}";
                    lbl_keyfound.Content = "✓ Key found";
                    lbl_keyfound.Foreground = Brushes.Green;
                    ValidateKey_FileName = keyPath;
                }
                else
                {
                    lbl_keydate.Content = "Created: --";
                    lbl_keyfound.Content = "× Key missing";
                    lbl_keyfound.Foreground = Brushes.Red;
                    btn_validatekey.IsEnabled = false;
                }
            }
            else
            {
                lbl_keydate.Content = "Created: --";
                lbl_keylocation.Content = $"Expected location: Not set";
                lbl_keylocation.Foreground = Brushes.Red;
                lbl_keyfound.Content = "× No key";
                lbl_keyfound.Foreground = Brushes.Red;
                btn_validatekey.IsEnabled = false;
            }
        }
        private void btn_newKey_click(object sender, RoutedEventArgs e)
        {
            btn_loadkey.IsEnabled = true;
            btn_validatekey.IsEnabled = true;
            btn_newkey.IsEnabled = false;
            lbl_details.Content = "Pick a password: ";
            lbl_details.Margin = Thickness.Parse("0,0,300,57");
            Mode = "NewKey";
            if (Height < 455)
            {
                var thread = new Thread(new ThreadStart(AnimateDownwards));
                thread.Start();
            }

        }

        private void ValidateKey(string path, string password)
        {
            var zipFailed = false;
            using (ZipFile zip = ZipFile.Read(path))
            {
                zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                zip.CompressionLevel = CompressionLevel.None;
                zip.Password = password;
                 
                try
                {
                    zip.ExtractAll(Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
                catch
                {
                    zipFailed = true;
                    lbl_validation.Content = "× Validation failed";
                    lbl_validation.Foreground = Brushes.Red;
                    if(File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval");
                    if(File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey");
                    if (Height > 455)
                    {
                        var thread = new Thread(new ThreadStart(AnimateUpwards));
                        thread.Start();
                    }
                }
            }

            if (!zipFailed)
            {
                if (File.ReadAllLines(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval")[0] ==
                    Cryptography.Encryption.HashPassword(password))
                {
                    lbl_validation.Content = "✓ Validation successful";
                    lbl_validation.Foreground = Brushes.Green;
                    if(File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval");
                    if(File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey");
                    var writer =
                        new StreamWriter(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "config.ini");
                    writer.WriteLine(path);
                    writer.Flush();
                    writer.Close();
                    UpdateUI();
                    if (Height > 455)
                    {
                        var thread = new Thread(new ThreadStart(AnimateUpwards));
                        thread.Start();
                    }
                }
                else
                {
                    lbl_validation.Content = "× Validation failed";
                    lbl_validation.Foreground = Brushes.Red;
                    if(File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval");
                    if(File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey");
                    if (Height > 455)
                    {
                        var thread = new Thread(new ThreadStart(AnimateUpwards));
                        thread.Start();
                    }
                }
            }
       
        }

      

        private async Task CreateNewKey(string password)
        {
            var dlg = new SaveFileDialog();
            var filter = new FileDialogFilter
            {
                Name = "Encryption package",
            };
            filter.Extensions.Add("epak");
            dlg.Filters.Add(filter);
            var _dlg = await dlg.ShowAsync(this);
            var result = false;
            var fileRes = "";
            fileRes = _dlg;
            File.WriteAllBytes($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}data.ekey",
                Cryptography.GenerateEncryptionKey(password));
            File.WriteAllText($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}conf.eval", Cryptography.Encryption.HashPassword(password));
            using(ZipFile zip = new ZipFile())
            {
                zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                zip.CompressionLevel = CompressionLevel.None;
                zip.Password = password;
                zip.AddFile($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}data.ekey", "");
                zip.AddFile($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}conf.eval", "");
                zip.Save(fileRes);
            }
            File.Delete($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}data.ekey");
            File.Delete($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}conf.eval");
            var thread = new Thread(new ThreadStart(AnimateUpwards));
            thread.Start();
            ValidateKey(fileRes, password);
        }

        private void AnimateDownwards()
        {
            var fullSize = false;
            do
            {
                Dispatcher.UIThread.Post(() =>
                {
                if (this.Height > 484)
                    fullSize = true;
                this.Height++;
                });
                  
                Thread.Sleep(1);
            } while (!fullSize);
            Dispatcher.UIThread.Post(() =>
            {
                lbl_details.IsVisible = true;
                lbl_enterkey.IsVisible = true;
                txt_pass.IsVisible = true;
                Height = 485;
               
            });
        }
        private void AnimateUpwards()
        {
            var fullSize = false;
            do
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (this.Height < 451)
                        fullSize = true;
                    this.Height--;
                });
                  
                Thread.Sleep(1);
               
            } while (!fullSize);
            Dispatcher.UIThread.Post(() =>
            {
                lbl_details.IsVisible = false;
                lbl_enterkey.IsVisible = false;
                txt_pass.IsVisible = false;
                btn_loadkey.IsEnabled = true;
                Height = 450;
                btn_newkey.IsEnabled = true;
                btn_validatekey.IsEnabled = true;
            });
        }

        private void txt_pass_gotFocus(object? sender, GotFocusEventArgs e)
        {
            BorderBrush = Brushes.Red;
        }

        private void Txt_pass_OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = false;
                if (Mode == "NewKey")
                {
                    CreateNewKey(txt_pass.Text);
                }
                else if (Mode == "LoadKey")
                {
                    ValidateKey(LoadKey_FileName, txt_pass.Text);
                }
                else if (Mode == "ValidateKey")
                {
                    ValidateKey(ValidateKey_FileName, txt_pass.Text);
                }
            }
        }

        private void Btn_loadkey_OnClick(object? sender, RoutedEventArgs e)
        {
      
            btn_loadkey.IsEnabled = false;
            btn_newkey.IsEnabled = true;
            btn_validatekey.IsEnabled = true;
            lbl_details.Content = "Enter password to load key:";
            lbl_details.Margin = Thickness.Parse("0,0,245,57");
            LoadKey();
        }

        private async Task LoadKey()
        {
          
            var dlg = new OpenFileDialog();
            var filter = new FileDialogFilter
            {
                Name = "Encryption key file",
            };
            filter.Extensions.Add("epak");
            dlg.Filters.Add(filter);
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
                LoadKey_FileName = fileRes;
                Mode = "LoadKey";
                var thread = new Thread(new ThreadStart(AnimateDownwards));
                thread.Start();
            }
        }

        private void Btn_validatekey_OnClick(object? sender, RoutedEventArgs e)
        {
            btn_loadkey.IsEnabled = true;
            btn_validatekey.IsEnabled = false;
            btn_newkey.IsEnabled = true;
            lbl_details.Content = "Enter password to validate key:";
            lbl_details.Margin = Thickness.Parse("0,0,225,57");
            if (Height < 455)
            {
                var thread = new Thread(new ThreadStart(AnimateDownwards));
                thread.Start();
            }
            Mode = "ValidateKey";
        }
    }
}