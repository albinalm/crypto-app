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
            if(args.Length > 2)
            {
                foreach(var file in args)
                {
                    if(!file.StartsWith("CryptoGUI.dll"))
                        EncryptionData.Sources.Add(file);
                }
            }
            else if(args.Length == 2)
            {
         //       File.WriteAllBytes(@"C:\users\albin\desktop\key.key", Cryptography.GenerateEncryptionKey(Cryptography.Encryption.HashPassword("ost123")));
                EncryptionData.SourceFileName = args[1];
                DecryptionData.SourceFileName = args[1];
                this.Hide();
                
                Encryptor encryptor = new Encryptor();
              //  encryptor.Show();
                
                Decryptor decryptor = new Decryptor();
               decryptor.Show();
            }
          
          
        }
  
    }
}