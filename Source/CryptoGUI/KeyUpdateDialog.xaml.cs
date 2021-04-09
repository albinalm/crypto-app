using System;
using System.Collections.Generic;
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
using CryptoAPI.ORM;
using Microsoft.Win32;

namespace CryptoGUI
{
    /// <summary>
    /// Interaction logic for KeyUpdateDialog.xaml
    /// </summary>
    public partial class KeyUpdateDialog : Window
    {
        public KeyUpdateDialog()
        {
            InitializeComponent();

        }
        private void WriteConfiguration()
        {
            if(File.Exists(Environment.CurrentDirectory + @"\credential"))
            {
                File.Decrypt(Environment.CurrentDirectory + @"\credential");
                var passwordHash = File.ReadAllText(Environment.CurrentDirectory + @"\credential");
                if (Cryptography.Encryption.HashPassword(txt_oldPw.Password) == passwordHash)
                {
                    if (txt_newPw.Password == txt_repeat.Password)
                    {
                        File.Delete(Environment.CurrentDirectory + @"\credential");
                        File.WriteAllText(Cryptography.Encryption.HashPassword(txt_newPw.Password), Environment.CurrentDirectory + @"\credential");
                        File.Encrypt(Environment.CurrentDirectory + @"\credential");
                        var dlg = new SaveFileDialog();
                        dlg.Filter = "Encryption key file|*.ekey";
                        if(dlg.ShowDialog() == true)
                        {
                            File.WriteAllBytes(dlg.FileName, Cryptography.GenerateEncryptionKey(Cryptography.Encryption.HashPassword(txt_newPw.Password)));
                            var writer = new StreamWriter(Environment.CurrentDirectory + @"\config.ini");
                            writer.WriteLine(dlg.FileName);
                            writer.Flush();
                            writer.Close();
                        }
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("The new passwords doesn't match", "Password mismatch", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Incorrect password", "Incorrect password", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Credential file not found. Will do new password without since this might be first time alpha usage");
                if (txt_newPw.Password == txt_repeat.Password)
                {
                    StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + @"\credential");
                    writer.WriteLine(Cryptography.Encryption.HashPassword(txt_newPw.Password));
                    writer.Flush();
                    writer.Close();
                
                    File.Encrypt(Environment.CurrentDirectory + @"\credential");
                    var dlg = new SaveFileDialog();
                    dlg.Filter = "Encryption key file|*.ekey";
                    if (dlg.ShowDialog() == true)
                    {
                        File.WriteAllBytes(dlg.FileName, Cryptography.GenerateEncryptionKey(Cryptography.Encryption.HashPassword(txt_newPw.Password)));
                        writer = new StreamWriter(Environment.CurrentDirectory + @"\config.ini");
                        writer.WriteLine(dlg.FileName);
                        writer.Flush();
                        writer.Close();
                    }
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("The new passwords doesn't match", "Password mismatch", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WriteConfiguration();;
        }
    }
}
