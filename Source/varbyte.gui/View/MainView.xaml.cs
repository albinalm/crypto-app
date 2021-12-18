using System.Windows;
using varbyte.encryption.Interfaces;
using varbyte.encryption.ORM;
using varbyte.gui.View.Dialogues;

namespace varbyte.gui.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private readonly ICryptography _cryptography;
        public MainView(ICryptography cryptography)
        {
            _cryptography = cryptography;
            InitializeComponent();
        }

        private void BtnGeneratekey_OnClick(object sender, RoutedEventArgs e)
        {
            var newKeyDlg = new NewKeyDlg(_cryptography);
            var dialogResult = newKeyDlg.ShowDialog();
            switch (dialogResult)
            {
                case true:
                    // User accepted dialog box
                    break;
                case false:
                    // User canceled dialog box
                    break;
                default:
                    // Indeterminate
                    break;
            }
        }
    }
}