using System;
using System.Diagnostics;
using System.IO;
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
            Activated += Configuration_Activated;
        }

        private void Configuration_Activated(object? sender, EventArgs e)
        {
            ReadData();
        }

        private void ReadData()
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
                    dlg.Filter = "Encryption key file|*.ekey";
                    if (dlg.ShowDialog() == true)
                    {
                        keyPath = dlg.FileName;
                        txt_pwHash.Text = hashReader.ReadLine();
                        File.Encrypt(Environment.CurrentDirectory + @"\credential");
                        txt_key.Text = keyPath;
                        _keyBytes = File.ReadAllBytes(txt_key.Text);
                        keyReader.Close();
                        hashReader.Close();
                        if (MessageBox.Show("Do you wish to save the path to the key file?", "Save path?",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
                        var writer = new StreamWriter(Environment.CurrentDirectory + @"\config.ini");
                        writer.WriteLine(dlg.FileName);
                        writer.Flush();
                        writer.Close();
                    }
                    else
                    {
                        MessageBox.Show("Data files are corrupt");
                    }
                }
            }
            else
            {
                MessageBox.Show("Data files are corrupt");
            }
        }

        private void txt_keyData_OnPointerEnter(object? sender, PointerEventArgs e)
        {
        }
    }
}