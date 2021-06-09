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
using System.Threading;
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
          
              Initialized += window_initialized;
               Activated += window_activated;
            
           
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif
            InitializeStartup();
        }

        private void AddSources(bool decryption)
        {
            for (var i = 0; i <= Environment.GetCommandLineArgs().Length - 1; i++)
            {
                var arg = Environment.GetCommandLineArgs()[i];
                if (i == 0 || arg is "CryptoApp_CommandArgs_Encrypt" or "CryptoApp_CommandArgs_Decrypt")
                    continue;
                if(!decryption)
                    EncryptionData.Sources.Add(Environment.GetCommandLineArgs()[i]);
                else 
                    DecryptionData.Sources.Add(Environment.GetCommandLineArgs()[i]);
            }
        }
        private async Task InitializeStartup()
        {
                   var args = Environment.GetCommandLineArgs();
            if (args.Length > 3)
            {
                if (args.Contains("CryptoApp_CommandArgs_Encrypt"))
                    try
                    {
                        AddSources(false); 
                        var encryptorArray = new EncryptorArray();
                        InitCryptography();
                        encryptorArray.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "MainWindow is faulty! " + ex, "Error in MainWindow", MessageBox.MessageBoxButtons.Ok);
                    }
                else if (args.Contains("CryptoApp_CommandArgs_Decrypt"))
                    try
                    {
                        AddSources(true);
                        var decryptorArray = new DecryptorArray();
                        await InitCryptography();

                        decryptorArray.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "MainWindow is faulty! " + ex, "Error in MainWindow", MessageBox.MessageBoxButtons.Ok);
                    }
            }
            else if (args.Length == 3)
            {
                if (args[1] == "CryptoApp_CommandArgs_Encrypt")
                {
                    await InitCryptography();
                    EncryptionData.SourceFileName = args[2];
                    var encryptor = new Encryptor();
                   
                    encryptor.Show();
                }
                else if (args[1] == "CryptoApp_CommandArgs_Decrypt")
                {
                    await InitCryptography();
                    DecryptionData.SourceFileName = args[2];
                    var decryptor = new Decryptor();
                    decryptor.Show();
                }
            }
            else
            {
               
                var conf = new Configuration();
                conf.Show();
                    //  this.Hide();
            }
        }
        private void window_lostFocus(object? sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void window_initialized(object? sender, EventArgs e)
        {
            
       
        }

        private void window_activated(object? sender, EventArgs e)
        {
     //       
            //Hide();
        }

        private void window_finishedInit(object? sender, EventArgs e)
        {
        }

        private void ShowMessage_Click(object sender, RoutedEventArgs e)
        {
             sfdlg();
        }

        public async Task sfdlg()
        {
            var dlg = new SaveFileDialog();
            var filter = new FileDialogFilter
            {
                Name = "Encryption key file",
            };
            filter.Extensions.Add("ekey");
            dlg.Filters.Add(filter);
            var _dlg =  await dlg.ShowAsync(this);
            var result = false;
            var fileRes = "";
            fileRes = _dlg;
        }
        private async Task InitCryptography()
        {
            if(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) != null)
                Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
          //  MessageBox.Show(this, Environment.CurrentDirectory, "", MessageBox.MessageBoxButtons.Ok);
            if (File.Exists(Environment.CurrentDirectory + @"/credential"))
            {
          //      File.Decrypt(Environment.CurrentDirectory + @"\credential");
                var keyReader = new StreamReader(Environment.CurrentDirectory + @"/config.ini");
                var hashReader = new StreamReader(Environment.CurrentDirectory + @"/credential");
                var keyPath = keyReader.ReadLine();
                var hash = hashReader.ReadLine();
                if (File.Exists(keyPath))
                {
                    //  Hide();
                    var pwDiag = new PasswordDialogue(hash);
                    await pwDiag.ShowDialog(this);
                    switch (pwDiag.Valid)
                    {
                        case true:
                            Cryptography.ReadEncryptionKey(pwDiag.OutputPw, File.ReadAllBytes(keyPath));
                            keyReader.Close();
                            hashReader.Close();
                       //     File.Encrypt(Environment.CurrentDirectory + @"\credential");
                            break;

                        case false:
                            await MessageBox.Show(this, "Incorrect credentials", "Incorrect credentials", MessageBox.MessageBoxButtons.Ok);
                       //     File.Encrypt(Environment.CurrentDirectory + @"\credential");
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
                          //      File.Encrypt(Environment.CurrentDirectory + @"\credential");
                                break;

                            case false:
                                await MessageBox.Show(this, "Incorrect credentials", "Incorrect credentials", MessageBox.MessageBoxButtons.Ok);
                           //     File.Encrypt(Environment.CurrentDirectory + @"\credential");
                                Environment.Exit(0);
                                break;

                            default:
                                await MessageBox.Show(this, "Incorrect credentials", "Incorrect credentials", MessageBox.MessageBoxButtons.Ok);
                                File.Encrypt(Environment.CurrentDirectory + @"\credential");
                                Environment.Exit(0);
                                break;
                        }
                    }
                    else
                    {
                        Show();
                        await MessageBox.Show(this, "Interrupted", "Interrupted", MessageBox.MessageBoxButtons.Ok);
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                await MessageBox.Show(this, "Did not find data files", "Did not find data files", MessageBox.MessageBoxButtons.Ok);
                Environment.Exit(0);
            }
        }
    }
}