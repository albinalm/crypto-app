using System.IO;
using System.Windows;
using System.Windows.Media;
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
                    CheckKey();
                    break;
                case false:
                    // User canceled dialog box
                    break;
                default:
                    // Indeterminate
                    break;
            }
        }

        private void MainView_OnLoaded(object sender, RoutedEventArgs e)
        {
       CheckKey();
        }

        private void CheckKey()
        {
            var primaryKeyPath = Services.CryptographyKeyService.GetPrimaryKeyPath();
            if (primaryKeyPath != null)
            {
                if (File.Exists(primaryKeyPath))
                {
                    LblKeyInfo.Content = "✓ Primary key found";
                    LblKeyPath.Content = primaryKeyPath;
                    LblKeyPath.Foreground = new SolidColorBrush(Colors.Green);
                    LblKeyPath.Visibility = Visibility.Visible;
                    LblKeyInfo.Foreground = new SolidColorBrush(Colors.Green);
                    LblKeyInfo.Margin = new Thickness(0, 100, 0, 0);
                    BtnGeneratekey.Visibility = Visibility.Hidden;
                    BtnAddKey.Visibility = Visibility.Visible;
                    BtnCheckKey.Visibility = Visibility.Visible;
                    BtnLoadKey.Visibility = Visibility.Visible;
                }
                else
                {
                    LblKeyInfo.Content = "⚠ Primary key missing";
                    LblKeyPath.Content = primaryKeyPath;
                    LblKeyPath.Foreground = new SolidColorBrush(Colors.DarkGoldenrod);
                    LblKeyPath.Visibility = Visibility.Visible;
                    LblKeyInfo.Foreground = new SolidColorBrush(Colors.DarkGoldenrod);
                    LblKeyInfo.Margin = new Thickness(0, 100, 0, 0);
                    BtnGeneratekey.Margin = new Thickness(0, 0, 0, 70);
                    BtnGeneratekey.Visibility = Visibility.Hidden;
                    BtnAddKey.Visibility = Visibility.Visible;
                    BtnCheckKey.Visibility = Visibility.Visible;
                    BtnLoadKey.Visibility = Visibility.Visible;
                }
            }
            else
            {
                LblKeyInfo.Content = "❌ No primary key set";
                LblKeyInfo.Foreground = new SolidColorBrush(Colors.Red);
                LblKeyPath.Visibility = Visibility.Hidden;
                LblKeyInfo.Margin = new Thickness(0, 120, 0, 0);
                BtnGeneratekey.Margin = new Thickness(0, 0, 0, 90);
                BtnGeneratekey.Visibility = Visibility.Visible;
                BtnAddKey.Visibility = Visibility.Hidden;
                BtnCheckKey.Visibility = Visibility.Hidden;
                BtnLoadKey.Visibility = Visibility.Hidden;
            }

        }

        private void BtnLoadKey_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void BtnCheckKey_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}