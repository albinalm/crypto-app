using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace CryptoGUI
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {

            InitializeComponent();
            Closing += Configuration_Closing;
            txt_key.IsReadOnly = true;
            txt_keyData.IsReadOnly = true;
            txt_pwHash.IsReadOnly = true;
            txt_keyData.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            Loaded += Configuration_Loaded;
        }
        private void Configuration_Loaded(object sender, RoutedEventArgs e)
        {
            ReadData();
        }
            private void Configuration_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }
        private void ReadData()
        {
            if (File.Exists(Environment.CurrentDirectory + @"\credential") && File.Exists(Environment.CurrentDirectory + @"\config.ini"))
            {
                
                File.Decrypt(Environment.CurrentDirectory + @"\credential");
                StreamReader keyReader = new StreamReader(Environment.CurrentDirectory + @"\config.ini");
                StreamReader hashReader = new StreamReader(Environment.CurrentDirectory + @"\credential");
                var keyPath = keyReader.ReadLine();
                if (File.Exists(keyPath))
                {
                    txt_pwHash.Text = hashReader.ReadLine();
                    File.Encrypt(Environment.CurrentDirectory + @"\credential");
                    txt_key.Text = keyPath;
                    var bytes = File.ReadAllBytes(txt_key.Text);
                    txt_keyData.Text = BitConverter.ToString(bytes);
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
                        var bytes = File.ReadAllBytes(txt_key.Text);
                        txt_keyData.Text = BitConverter.ToString(bytes);
                        keyReader.Close();
                        hashReader.Close();
                        if (MessageBox.Show("Do you wish to save the path to the key file?", "Save path?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            var writer = new StreamWriter(Environment.CurrentDirectory + @"\config.ini");
                            writer.WriteLine(dlg.FileName);
                            writer.Flush();
                            writer.Close();
                        }
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

        private void btn_generateKey_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                "Changing these values will make your previous archives inaccessible.\nIf you wish to keep your data, please decrypt them first and then re-encrypt them after " +
                "data has been updated.\n\n Are you sure you wish to proceed?", "Credential update warning.", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            var updateKeyDlg = new KeyUpdateDialog();
            var dialogResult = updateKeyDlg.ShowDialog();
          
            switch(dialogResult)
            {
                case true:
                    ReadData();
                    break;
            }
        }
    }
}
