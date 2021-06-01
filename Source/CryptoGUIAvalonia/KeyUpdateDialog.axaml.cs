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
    public class KeyUpdateDialog : Window
    {
        private TextBox txt_newPw { get; set; }
        private TextBox txt_repeat { get; set; }
        private TextBox txt_oldPw { get; set; }
        public bool isDone { get; set; }

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
            txt_newPw = this.Get<TextBox>("txt_newPw");
            txt_oldPw = this.Get<TextBox>("txt_oldPw");
            txt_repeat = this.Get<TextBox>("txt_repeat");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WriteConfiguration();
        }

        private async Task WriteConfiguration()
        {
            if (File.Exists(Environment.CurrentDirectory + @"\credential"))
            {
                File.Decrypt(Environment.CurrentDirectory + @"\credential");
                var passwordHash = File.ReadAllText(Environment.CurrentDirectory + @"\credential");
                if (Cryptography.Encryption.HashPassword(txt_oldPw.Text) == passwordHash)
                {
                    if (txt_newPw.Text == txt_repeat.Text)
                    {
                        File.Delete(Environment.CurrentDirectory + @"\credential");
                        File.WriteAllText(Cryptography.Encryption.HashPassword(txt_newPw.Text),
                            Environment.CurrentDirectory + @"\credential");
                        File.Encrypt(Environment.CurrentDirectory + @"\credential");

                        var dlg = new SaveFileDialog();
                        var filter = new FileDialogFilter
                        {
                            Name = "Encryption key file",
                        };
                        filter.Extensions.Add("ekey");
                        dlg.Filters.Add(filter);
                        var _dlg = dlg.ShowAsync(this);
                        var result = false;
                        var fileRes = "";
                        fileRes = _dlg.Result;

                        if (!string.IsNullOrWhiteSpace(fileRes))
                        {
                            File.WriteAllBytes(fileRes,
                                Cryptography.GenerateEncryptionKey(
                                    Cryptography.Encryption.HashPassword(txt_newPw.Text)));
                            var writer = new StreamWriter(Environment.CurrentDirectory + @"\config.ini");
                            writer.WriteLine(fileRes);
                            writer.Flush();
                            writer.Close();
                        }
                        isDone = true;
                        Close();
                    }
                    else
                    {
                        await MessageBox.Show(this, "The new passwords doesn't match", "Password mismatch", MessageBox.MessageBoxButtons.Ok);
                    }
                }
                else
                {
                    await MessageBox.Show(this, "The new passwords doesn't match", "Password mismatch", MessageBox.MessageBoxButtons.Ok);
                }
            }
            else
            {
                await MessageBox.Show(this, "Credential file not found. Will do new password without since this might be first time alpha usage", "Credential file not found", MessageBox.MessageBoxButtons.Ok);
                if (txt_newPw.Text == txt_repeat.Text)
                {
                    var writer = new StreamWriter(Environment.CurrentDirectory + @"\credential");
                    writer.WriteLine(Cryptography.Encryption.HashPassword(txt_newPw.Text));
                    writer.Flush();
                    writer.Close();

                    File.Encrypt(Environment.CurrentDirectory + @"\credential");
                    var dlg = new SaveFileDialog();
                    var filter = new FileDialogFilter
                    {
                        Name = "Encryption key file",
                    };
                    filter.Extensions.Add("ekey");
                    dlg.Filters.Add(filter);
                    var _dlg = await dlg.ShowAsync(this);
                    var result = false;
                    var fileRes = "";
                    fileRes = _dlg;
                    if (!string.IsNullOrWhiteSpace(fileRes))
                    {
                        File.WriteAllBytes(fileRes,
                            Cryptography.GenerateEncryptionKey(
                                Cryptography.Encryption.HashPassword(txt_newPw.Text)));
                        writer = new StreamWriter(Environment.CurrentDirectory + @"\config.ini");
                        writer.WriteLine(fileRes);
                        writer.Flush();
                        writer.Close();
                    }

                    isDone = true;
                    Close();
                }
                else
                {
                    await MessageBox.Show(this, "The new passwords doesn't match", "Password mismatch", MessageBox.MessageBoxButtons.Ok);
                }
            }
        }
    }
}