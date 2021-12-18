using System.Windows;
using varbyte.encryption.Interfaces;
using varbyte.encryption.ORM;
using varbyte.encryption.Service;
using varbyte.gui.View.Dialogues;

namespace varbyte.gui.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private readonly Services Services;
        public MainView(Services services)
        {
            Services = services;
            InitializeComponent();
        }

        private void BtnGeneratekey_OnClick(object sender, RoutedEventArgs e)
        {
            var newKeyDlg = new NewKeyDlg(Services);
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