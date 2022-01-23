using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using varbyte.encryption.Service;
using varbyte.gui.View.Dialogues;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace varbyte.gui.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private readonly string _baseDir;
        private readonly Services Services;

        public MainView(Services services)
        {
            Services = services;
            InitializeComponent();
#pragma warning disable CS8601 // Possible null reference assignment.
            _baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        private void BtnGeneratekey_OnClick(object sender, RoutedEventArgs e)
        {
            var newKeyDlg = new NewKeyDlg(Services);
            switch (newKeyDlg.ShowDialog())
            {
                case true:
                    CheckKey();
                    LblKeyInfo.Content = "✓ Key created, validated & loaded!";
                    break;
                case false:
                    if(!newKeyDlg.Canceled)
                    {
                        LblKeyInfo.Content = "❌ Key could not be created";
                        LblKeyPath.Content = "We tried 5 times, but we failed every time. Try selecting another destination";
                        LblKeyPath.Foreground = new SolidColorBrush(Colors.Red);
                        LblKeyPath.Visibility = Visibility.Visible;
                        LblKeyInfo.Foreground = new SolidColorBrush(Colors.Red);
                        LblKeyInfo.Margin = new Thickness(0, 100, 0, 0);
                        BtnGeneratekey.Visibility = Visibility.Hidden;
                        BtnAddKey.Visibility = Visibility.Visible;
                        BtnCheckKey.Visibility = Visibility.Visible;
                        BtnLoadKey.Visibility = Visibility.Visible;
                    }
                    
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
            var primaryKeyPath = Services.ConfigurationManager.Globals.PrimaryKeyPath;
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
            var openKeyDlg = new OpenKeyDlg(Services, true);
            if(!openKeyDlg.DialogCanceled)
            {
                switch (openKeyDlg.ShowDialog())
                {
                    case true:
                        CheckKey();
                        LblKeyInfo.Content = "✓ Key successfully loaded";
                        break;
                    case false:
                        if(!openKeyDlg.Canceled)
                        {
                            LblKeyInfo.Content = "❌ Key not loaded";
                            LblKeyPath.Content = "Something went wrong when trying to validate the key :(";
                            LblKeyPath.Foreground = new SolidColorBrush(Colors.Red);
                            LblKeyPath.Visibility = Visibility.Visible;
                            LblKeyInfo.Foreground = new SolidColorBrush(Colors.Red);
                            LblKeyInfo.Margin = new Thickness(0, 100, 0, 0);
                            BtnGeneratekey.Visibility = Visibility.Hidden;
                            BtnAddKey.Visibility = Visibility.Visible;
                            BtnCheckKey.Visibility = Visibility.Visible;
                            BtnLoadKey.Visibility = Visibility.Visible;
                        }
                           
                        break;
                    default:
                        // Indeterminate
                        break;
                }
            }
          
        }

        private void BtnCheckKey_OnClick(object sender, RoutedEventArgs e)
        {
            var openKeyDlg = new OpenKeyDlg(Services, false);
            if (!openKeyDlg.DialogCanceled)
            {
                switch (openKeyDlg.ShowDialog())
                {
                    case true:
                        LblKeyInfo.Content = "✓ Validation successful";
                        LblKeyPath.Content = "The key works like a charm!";
                        LblKeyPath.Foreground = new SolidColorBrush(Colors.Green);
                        LblKeyPath.Visibility = Visibility.Visible;
                        LblKeyInfo.Foreground = new SolidColorBrush(Colors.Green);
                        LblKeyInfo.Margin = new Thickness(0, 100, 0, 0);
                        BtnGeneratekey.Visibility = Visibility.Hidden;
                        BtnAddKey.Visibility = Visibility.Visible;
                        BtnCheckKey.Visibility = Visibility.Visible;
                        BtnLoadKey.Visibility = Visibility.Visible;

                        break;
                    case false:
                        if (!openKeyDlg.Canceled)
                        {
                            LblKeyInfo.Content = "❌ Validation failed";
                            LblKeyPath.Content = "Something went wrong when trying to validate the key :(";
                            LblKeyPath.Foreground = new SolidColorBrush(Colors.Red);
                            LblKeyPath.Visibility = Visibility.Visible;
                            LblKeyInfo.Foreground = new SolidColorBrush(Colors.Red);
                            LblKeyInfo.Margin = new Thickness(0, 100, 0, 0);
                            BtnGeneratekey.Visibility = Visibility.Hidden;
                            BtnAddKey.Visibility = Visibility.Visible;
                            BtnCheckKey.Visibility = Visibility.Visible;
                            BtnLoadKey.Visibility = Visibility.Visible;
                        }
                           

                        break;
                    default:
                        // Indeterminate
                        break;
                }
            }
            
        }
    }
}