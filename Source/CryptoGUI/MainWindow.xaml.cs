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
   
            if(args.Length > 3)
            {
                
                if(args.Contains("CryptoApp_CommandArgs_Encrypt"))
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
                        Cryptography.ReadEncryptionKey(Cryptography.Encryption.HashPassword("ost123"), File.ReadAllBytes(@"C:\users\albin\desktop\key.key"));
                        encryptorArray.Show();
                        this.Hide();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("MainWindow is faulty! " + ex.ToString());
                    }
                 
                  
                  
                }
                else if(args.Contains("CryptoApp_CommandArgs_Decrypt"))
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
                        Cryptography.ReadEncryptionKey(Cryptography.Encryption.HashPassword("ost123"), File.ReadAllBytes(@"C:\users\albin\desktop\key.key"));
                        decryptorArray.Show();
                        this.Hide();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("MainWindow is faulty! " + ex.ToString());
                    }
                }
            }
            else if(args.Length == 3)
            {
            
                if(args[1] == "CryptoApp_CommandArgs_Encrypt")
                {
                    EncryptionData.SourceFileName = args[2];
                    Encryptor encryptor = new Encryptor();
                    Cryptography.ReadEncryptionKey(Cryptography.Encryption.HashPassword("ost123"), File.ReadAllBytes(@"C:\users\albin\desktop\key.key"));
                    encryptor.Show();
                    this.Hide();
                }
               else if (args[1] == "CryptoApp_CommandArgs_Decrypt")
                {
                    DecryptionData.SourceFileName = args[2];
                    this.Hide();
                    Decryptor decryptor = new Decryptor();
                    decryptor.Show();
                }
            }
            MessageBox.Show("Invalid input args.");
            Environment.Exit(0);
        }
  
    }
}