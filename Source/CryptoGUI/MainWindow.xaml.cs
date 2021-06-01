using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using CryptoAPI.ORM;
using CryptoGUI.DataModel;
using Microsoft.Win32;

namespace CryptoGUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
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
                Hide();
                var conf = new Configuration();
                conf.Show();
            }
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void InitCryptography()
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
                    Hide();
                    var pwDiag = new PasswordDialogue(hash);
                    var dialogueResult = pwDiag.ShowDialog();

                    switch (dialogueResult)
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
                    Hide();
                    var dlg = new OpenFileDialog { Filter = "Encryption key file|*.ekey" };
                    if (dlg.ShowDialog() == true)
                    {
                        keyPath = dlg.FileName;
                        var pwDiag = new PasswordDialogue(hash);
                        var dialogueResult = pwDiag.ShowDialog();
                        switch (dialogueResult)
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