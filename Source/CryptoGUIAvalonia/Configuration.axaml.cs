using System;
using System.ComponentModel;
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
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;

namespace CryptoGUIAvalonia
{
    public class Configuration : Window
    {
        private TextBox txt_pwHash { get; set; }
        private TextBox txt_key { get; set; }
        private TextBox txt_keyData { get; set; }
        private Button btn_view_data { get; set; }
        private byte[] _keyBytes;

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
            Closing += Configuration_Closing;
            txt_key = this.Get<TextBox>("txt_key");
            txt_pwHash = this.Get<TextBox>("txt_pwHash");
            txt_keyData = this.Get<TextBox>("txt_keyData");
            btn_view_data = this.Get<Button>("btn_view_data");
            ReadData();
        }

        private void Configuration_Closing(object? sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Configuration_Activated(object? sender, EventArgs e)
        {
        }

        private async Task ReadData()
        {
            if (File.Exists(Environment.CurrentDirectory + @"\credential") &&
                File.Exists(Environment.CurrentDirectory + @"\config.ini"))
            {
                File.Decrypt(Environment.CurrentDirectory + @"\credential");
                var keyReader = new StreamReader(Environment.CurrentDirectory + @"\config.ini");
                var hashReader = new StreamReader(Environment.CurrentDirectory + @"\credential");
                var keyPath = keyReader.ReadLine();
                if (File.Exists(keyPath))
                {
                    txt_pwHash.Text = hashReader.ReadLine();
                    File.Encrypt(Environment.CurrentDirectory + @"\credential");
                    txt_key.Text = keyPath;
                    _keyBytes = File.ReadAllBytes(txt_key.Text);
                    keyReader.Close();
                    hashReader.Close();
                }
                else
                {
                    var dlg = new OpenFileDialog();
                    var filter = new FileDialogFilter
                    {
                        Name = "Encryption key file",
                    };
                    filter.Extensions.Add("ekey");
                    dlg.Filters.Add(filter);
                    var _dlg = dlg.ShowAsync(this);
                    var result = false;
                    var fileRes = "";
                    foreach (var res in await _dlg)
                    {
                        if (res.EndsWith(".ekey"))
                        {
                            fileRes = res;
                            result = true;
                        }
                    }
                    if (result)
                    {
                        keyPath = fileRes;
                        txt_pwHash.Text = hashReader.ReadLine();
                        File.Encrypt(Environment.CurrentDirectory + @"\credential");
                        txt_key.Text = keyPath;
                        _keyBytes = File.ReadAllBytes(txt_key.Text);
                        keyReader.Close();
                        hashReader.Close();
                        await MessageBox.Show(this, "Do you wish to save the path to the key file?", "Save path?", MessageBox.MessageBoxButtons.YesNo);

                        var writer = new StreamWriter(Environment.CurrentDirectory + @"\config.ini");
                        writer.WriteLine(fileRes);
                        writer.Flush();
                        writer.Close();
                    }
                    else
                    {
                        await MessageBox.Show(this, "Data files are corrupt", "Files corrupt", MessageBox.MessageBoxButtons.Ok);
                    }
                }
            }
            else
            {
                await MessageBox.Show(this, "Data files are corrupt", "Files corrupt", MessageBox.MessageBoxButtons.Ok);
            }
        }

        private void btn_generateKey_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("loL");
            onClick();
        }

        private async Task onClick()
        {
            await MessageBox.Show(this, "Changing these values will make your previous archives inaccessible.\nIf you wish to keep your data, please decrypt them first and then re-encrypt them after " +
              "data has been updated.\n\n Are you sure you wish to proceed?", "Credential update warning.", MessageBox.MessageBoxButtons.YesNo);

            var updateKeyDlg = new KeyUpdateDialog();
            await updateKeyDlg.ShowDialog(this);

            switch (updateKeyDlg.isDone)
            {
                case true:
                    ReadData();
                    break;
            }
        }

        private void btn_view_data_Click(object sender, RoutedEventArgs e)
        {
            txt_keyData.Text = BitConverter.ToString(_keyBytes);
            btn_view_data.IsVisible = false;
        }
    }
}