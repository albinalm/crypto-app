using CryptoAPI.ORM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using CryptoGUI.DataModel;
using Microsoft.Win32;

namespace CryptoGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Length > 3)
            {

                if (args.Contains("CryptoApp_CommandArgs_Encrypt"))
                {
                    try
                    {
                        foreach (var file in args)
                        {
                            if (!file.EndsWith("CryptoGUI.dll") && file != "CryptoApp_CommandArgs_Encrypt")
                            {
                                EncryptionData.Sources.Add(file);
                            }
                        }
                        EncryptorArray encryptorArray = new EncryptorArray();
                        InitCryptography();
                        encryptorArray.Show();
                   
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("MainWindow is faulty! " + ex.ToString());
                    }



                }
                else if (args.Contains("CryptoApp_CommandArgs_Decrypt"))
                {
                    try
                    {
                        foreach (var file in args)
                        {
                            if (!file.EndsWith("CryptoGUI.dll") && file != "CryptoApp_CommandArgs_Decrypt")
                            {
                                DecryptionData.Sources.Add(file);
                            }
                        }
                        DecryptorArray decryptorArray = new DecryptorArray();
                        InitCryptography();
                        
                        decryptorArray.Show();
                     
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("MainWindow is faulty! " + ex.ToString());
                    }
                }
            }
            else if (args.Length == 3)
            {

                if (args[1] == "CryptoApp_CommandArgs_Encrypt")
                {
                    EncryptionData.SourceFileName = args[2];
                    Encryptor encryptor = new Encryptor();
                    InitCryptography();
                    encryptor.Show();
                
                }
                else if (args[1] == "CryptoApp_CommandArgs_Decrypt")
                {
                    DecryptionData.SourceFileName = args[2];
                    Decryptor decryptor = new Decryptor();
                    InitCryptography();
                    decryptor.Show();  
      
                }
            }
            else
            {
                this.Hide();
                Configuration conf = new Configuration();
                conf.Show();
           
            }

        }
        private void InitCryptography()
        {
            
            Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (File.Exists(Environment.CurrentDirectory + @"\credential"))
            {
                File.Decrypt(Environment.CurrentDirectory + @"\credential");
                StreamReader keyReader = new StreamReader(Environment.CurrentDirectory + @"\config.ini");
                StreamReader hashReader = new StreamReader(Environment.CurrentDirectory + @"\credential");
                var keyPath = keyReader.ReadLine();
                var hash = hashReader.ReadLine();
                if (File.Exists(keyPath))
                {
                    this.Hide();
                    var pwDiag = new PasswordDialogue(hash);
                    var dialogueResult = pwDiag.ShowDialog();

                    switch (dialogueResult)
                    {
                        case true:
                            Cryptography.ReadEncryptionKey(pwDiag.OutputPW, File.ReadAllBytes(keyPath));
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
                    this.Hide();
                    var dlg = new OpenFileDialog();
                    dlg.Filter = "Encryption key file|*.ekey";
                    if(dlg.ShowDialog() == true)
                    {
                        keyPath = dlg.FileName;
                        var pwDiag = new PasswordDialogue(hash);
                        var dialogueResult = pwDiag.ShowDialog();
                        switch (dialogueResult)
                        {
                            case true:
                                Cryptography.ReadEncryptionKey(pwDiag.OutputPW, File.ReadAllBytes(keyPath));
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
                        this.Show();
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