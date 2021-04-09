using CryptoAPI.ORM;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for PasswordDialogue.xaml
    /// </summary>
    
    public partial class PasswordDialogue : Window
    {
        private string inputHash = "";
        public string OutputPW = "";
        public PasswordDialogue(string InputHash)
        {
            inputHash = InputHash;
            InitializeComponent();
        }

        private void btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            if(Cryptography.Encryption.HashPassword(txt_pw.Password) == inputHash)
            {
                OutputPW = txt_pw.Password;
                DialogResult = true;
                Close();
            }
            else
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
