using System.Windows;
using CryptoAPI.ORM;

namespace CryptoGUI
{
    /// <summary>
    ///     Interaction logic for PasswordDialogue.xaml
    /// </summary>
    public partial class PasswordDialogue : Window
    {
        private readonly string _inputHash;
        public string OutputPw = "";

        public PasswordDialogue(string inputHash)
        {
            _inputHash = inputHash;
            InitializeComponent();
        }

        private void btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            if (Cryptography.Encryption.HashPassword(txt_pw.Password) == _inputHash)
            {
                OutputPw = txt_pw.Password;
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