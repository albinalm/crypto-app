using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CryptoAPI.ORM;
using CryptoGUI.DataModel;
using CryptoGUIAvalonia.GUI;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CryptoGUIAvalonia
{
    public class MainWindow : Window
    {
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public MainWindow()
        {
            Debug.WriteLine("Fired!");
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 3)
            {
                if (args.Contains("CryptoApp_CommandArgs_Encrypt"))
                    try
                    {
                        foreach (var file in args)
                            if (!file.EndsWith("CryptoGUI.dll") && file != "CryptoApp_CommandArgs_Encrypt")
                                EncryptionData.Sources.Add(file);
                        var encryptorArray = new EncryptorArray();
                        InitCryptography();
                        encryptorArray.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("MainWindow is faulty! " + ex);
                    }
                else if (args.Contains("CryptoApp_CommandArgs_Decrypt"))
                    try
                    {
                        foreach (var file in args)
                            if (!file.EndsWith("CryptoGUI.dll") && file != "CryptoApp_CommandArgs_Decrypt")
                                DecryptionData.Sources.Add(file);
                        var decryptorArray = new DecryptorArray();
                        InitCryptography();

                        decryptorArray.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("MainWindow is faulty! " + ex);
                    }
            }
            else if (args.Length == 3)
            {
                if (args[1] == "CryptoApp_CommandArgs_Encrypt")
                {
                    EncryptionData.SourceFileName = args[2];
                    var encryptor = new Encryptor();
                    InitCryptography();
                    encryptor.Show();
                }
                else if (args[1] == "CryptoApp_CommandArgs_Decrypt")
                {
                    DecryptionData.SourceFileName = args[2];
                    var decryptor = new Decryptor();
                    InitCryptography();
                    decryptor.Show();
                }
            }
            else
            {
                var conf = new Configuration();
                conf.Show();
          //      this.Hide();
            }
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Activated += window_activated;
        }

        private void window_activated(object? sender, EventArgs e)
        {
            Hide();
        }

        private void window_finishedInit(object? sender, EventArgs e)
        {
        }

        private void ShowMessage_Click(object sender, RoutedEventArgs e)
        {
        }

        private async Task InitCryptography()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (File.Exists(Environment.CurrentDirectory + @"\credential"))
            {
                File.Decrypt(Environment.CurrentDirectory + @"\credential");
                var keyReader = new StreamReader(Environment.CurrentDirectory + @"\config.ini");
                var hashReader = new StreamReader(Environment.CurrentDirectory + @"\credential");
                var keyPath = keyReader.ReadLine();
                var hash = hashReader.ReadLine();
                if (File.Exists(keyPath))
                {
                  //  Hide();
                    var pwDiag = new PasswordDialogue(hash);
                    switch (pwDiag.Valid)
                    {
                        case true:
                            Cryptography.ReadEncryptionKey(pwDiag.OutputPw, File.ReadAllBytes(keyPath));
                            keyReader.Close();
                            hashReader.Close();
                            File.Encrypt(Environment.CurrentDirectory + @"\credential");
                            break;

                        case false:
                            MessageBox.Show("Incorrect credentials");
                            File.Encrypt(Environment.CurrentDirectory + @"\credential");
                            Environment.Exit(0);
                            break;

                        default:
                    }
                }
                else
                {
                   // Hide();
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
                    if (result == true)
                    {
                        keyPath = fileRes;

                        var pwDiag = new PasswordDialogue(hash);
                        switch (pwDiag.Valid)
                        {
                            case true:
                                Cryptography.ReadEncryptionKey(pwDiag.OutputPw, File.ReadAllBytes(keyPath));
                                keyReader.Close();
                                hashReader.Close();
                                File.Encrypt(Environment.CurrentDirectory + @"\credential");
                                break;

                            case false:
                                MessageBox.Show("Incorrect credentials");
                                File.Encrypt(Environment.CurrentDirectory + @"\credential");
                                Environment.Exit(0);
                                break;

                            default:
                                MessageBox.Show("Incorrect credentials");
                                File.Encrypt(Environment.CurrentDirectory + @"\credential");
                                Environment.Exit(0);
                                break;
                        }
                    }
                    else
                    {
                        Show();
                        MessageBox.Show("Interrupted");
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                MessageBox.Show("Did not find data files");
                Environment.Exit(0);
            }
        }
    }
}