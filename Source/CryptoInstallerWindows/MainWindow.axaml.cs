using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryptoTranslation;
using Ionic.Zip;
using IWshRuntimeLibrary;

namespace CryptoInstallerWindows
{
    public class MainWindow : Window
    {
        private Dict Dictionary { get; set; }

        private bool isResizing = false;

        private Panel FirstPane;
        private Panel SecondPane;
        private Panel ThirdPane;

        private Image img_icon;
        private Image img_icon2;
        private Image img_icon3;
        private Image img_example;

        private Label lbl_question;
        private Label lbl_welcome;
        private Label lbl_welcome2;
        private Label lbl_path;
        private Label lbl_help;
        private Label lbl_status;
        private Label lbl_finished;

        private Border border_fm;

        private Button btn_next;
        private Button btn_back;
        private Button btn_install;
        private Button btn_finish;
        private Button btn_path;

        private TextBox txt_path;

        private ProgressBar pb_progress;

        private CheckBox chk_start;
        private bool kde = true;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public MainWindow()
        {
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif

            lbl_welcome = this.Get<Label>("lbl_welcome");
            lbl_welcome2 = this.Get<Label>("lbl_welcome2");
            img_icon = this.Get<Image>("img_icon");
            img_icon2 = this.Get<Image>("img_icon2");
            img_icon3 = this.Get<Image>("img_icon3");

            FirstPane = this.Get<Panel>("FirstPane");
            SecondPane = this.Get<Panel>("SecondPane");
            lbl_path = this.Get<Label>("lbl_path");
            btn_back = this.Get<Button>("btn_back");
            lbl_help = this.Get<Label>("lbl_help");

            btn_install = this.Get<Button>("btn_install");
            pb_progress = this.Get<ProgressBar>("pb_progress");
            img_example = this.Get<Image>("img_example");
            chk_start = this.Get<CheckBox>("chk_start");
            btn_finish = this.Get<Button>("btn_finish");
            lbl_status = this.Get<Label>("lbl_status");
            btn_path = this.Get<Button>("btn_path");
            ThirdPane = this.Get<Panel>("ThirdPane");
            lbl_finished = this.Get<Label>("lbl_finished");
            img_icon.Source = ReadEmbeddedResourceImage("logo01.png");
            img_icon2.Source = ReadEmbeddedResourceImage("logo01.png");
            img_icon3.Source = ReadEmbeddedResourceImage("logo01.png");

            btn_next = this.Get<Button>("btn_next");

            txt_path = this.Get<TextBox>("txt_path");

            txt_path.Text = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\CodeIndite\Privateer";
        }

        private Bitmap ReadEmbeddedResourceImage(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("CryptoInstallerWindows.Resources." + resourceName))
            {
                if (stream != null) return new Bitmap(stream);
            }
            return null;
        }

        private void AnimateDownwards()
        {
            var fullSize = false;
            do
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    this.Height += 6;
                    if (this.Height >= 900)
                    {
                        fullSize = true;
                        this.Height = 900;
                    }
                    this.Position = new PixelPoint(this.Position.X, this.Position.Y - 3);
                });

                Thread.Sleep(1);
            } while (!fullSize);

            var thread = new Thread(new ThreadStart(FadeInHelpElements));
            thread.Start();
        }

        private void FadeInHelpElements()
        {
            var isFaded = false;
            do
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    img_example.Opacity += 0.02;
                    lbl_help.Opacity += 0.02;
                    if (img_example.Opacity >= 1)
                    {
                        isFaded = true;
                        img_example.Opacity = 1;
                        lbl_help.Opacity = 1;
                        var thread = new Thread(new ThreadStart(FadeInLastElements));
                        thread.Start();
                    }
                });
                Thread.Sleep(1);
            } while (!isFaded);
        }

        private void FadeInLastElements()
        {
            var isFaded = false;
            do
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    btn_finish.Opacity += 0.1;
                    if (btn_finish.Opacity >= 1)
                    {
                        isFaded = true;

                        btn_finish.Opacity = 1;
                    }
                });
                Thread.Sleep(1);
            } while (!isFaded);

            FadeInButton();
        }

        private void WriteInstallFile()
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("CryptoInstallerWindows.InstallData.install_windows.zip"))
            {
                using (var file = new FileStream(AppContext.BaseDirectory + @"/install_windows.zip", FileMode.Create, FileAccess.Write))
                {
                    resource?.CopyTo(file);
                }
            }

            var thread = new Thread(new ThreadStart(Extract));
            thread.IsBackground = true;
            thread.Start();
        }

        private void ExecuteWait()
        {
            var thread = new Thread(new ThreadStart(Wait));
            thread.Start();
        }

        private void Wait()
        {
            Thread.Sleep(500);
            var thread = new Thread(new ThreadStart(FadeIn));
            thread.Start();
        }

        private void FadeIn()
        {
            var isFaded = false;
            do
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    lbl_welcome.Opacity += 0.02;
                    img_icon.Opacity += 0.02;
                    if (lbl_welcome.Opacity >= 1)
                    {
                        isFaded = true;
                        lbl_welcome.Opacity = 1;
                        img_icon.Opacity = 1;
                    }
                });
                Thread.Sleep(1);
            } while (!isFaded);

            Thread.Sleep(100);
            FadeInButton();
        }

        private void FadeInButton()
        {
            var isFaded = false;
            do
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    btn_next.Opacity += 0.1;
                    if (btn_next.Opacity >= 1)
                    {
                        isFaded = true;
                        btn_next.Opacity = 1;
                    }
                });
                Thread.Sleep(1);
            } while (!isFaded);
        }

        private void InitializeTranslation()
        {
            var language = System.Globalization.CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName;
            var engine = new TranslationEngine();

            Dictionary = engine.InitializeLanguage(TranslationEngine.Languages.Contains(language) ? language : "eng");
            lbl_welcome.Content = Dictionary.InstallerWindows_WelcomeTo;
            lbl_welcome2.Content = Dictionary.InstallerWindows_WelcomeTo;

            btn_next.Content = Dictionary.InstallerWindows_Next;
            btn_back.Content = Dictionary.InstallerWindows_Back;
            lbl_path.Content = Dictionary.InstallerWindows_WhereToInstall;
            btn_install.Content = Dictionary.InstallerWindows_Install;
            Title = Dictionary.InstallerWindows_Title;
            lbl_help.Content = Dictionary.InstallerWindows_UseDescription;
            lbl_status.Content = Dictionary.InstallerWindows_Installing;
            btn_finish.Content = Dictionary.InstallerWindows_Finish;
            chk_start.Content = Dictionary.InstallerWindows_OpenPrivateer;

            lbl_finished.Content = Dictionary.InstallerWindows_FinishedInstalling;
            if (TranslationEngine.Languages.Contains(language))
                img_example.Source = ReadEmbeddedResourceImage("example_" + language + ".png");
            else
                img_example.Source = ReadEmbeddedResourceImage("example_eng" + ".png");
            ExecuteWait();
        }

        private void WindowBase_OnActivated(object? sender, EventArgs e)
        {
            InitializeTranslation();
        }

        private void Btn_next_OnClick(object? sender, RoutedEventArgs e)
        {
            FirstPane.IsVisible = false;
            SecondPane.IsVisible = true;
        }

        private void Btn_back_OnClick(object? sender, RoutedEventArgs e)
        {
            FirstPane.IsVisible = true;
            SecondPane.IsVisible = false;
        }

        private void Btn_path_OnClick(object? sender, RoutedEventArgs e)
        {
            ShowFolderDialog();
        }

        private async Task ShowFolderDialog()
        {
            OpenFolderDialog dlg = new OpenFolderDialog();
            var result = await dlg.ShowAsync(this);
            txt_path.Text = @$"{result}\Privateer";
        }

        private void Btn_install_OnClick(object? sender, RoutedEventArgs e)
        {
            Title = Dictionary.InstallerWindows_TitleInstalling;
            btn_install.IsEnabled = false;
            btn_back.IsEnabled = false;
            txt_path.IsEnabled = false;
            btn_path.IsEnabled = false;
            lbl_status.IsVisible = true;
            pb_progress.Maximum = 10;
            pb_progress.Value = 2;
            var thread = new Thread(new ThreadStart(WriteInstallFile));
            thread.IsBackground = true;
            thread.Start();
        }

        private void Extract()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                pb_progress.Value = 4;
            });

            if (!Directory.Exists(txt_path.Text))
                Directory.CreateDirectory(txt_path.Text);
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                pb_progress.Value = 5;
            });

            using (var zip = ZipFile.Read(AppContext.BaseDirectory + @"/install_windows.zip"))
            {
                zip.ExtractAll(txt_path.Text);
            }
            System.IO.File.Delete(AppContext.BaseDirectory + @"/install_windows.zip");
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                pb_progress.Value = 10;
                SecondPane.IsVisible = false;
                ThirdPane.IsVisible = true;
                Title = Dictionary.InstallerWindows_TitleFinish;
            });
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("CryptoInstallerWindows.InstallData.icon.ico"))
            {
                using (var file = new FileStream(txt_path.Text + @"/icon.ico", FileMode.Create, FileAccess.Write))
                {
                    resource?.CopyTo(file);
                }
            }
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"/CodeIndite"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"/CodeIndite");

            Windows_WriteShortcut($@"{Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)}\CodeIndite\Privateer.lnk");
            Windows_WriteShortcut($@"{Environment.GetFolderPath(Environment.SpecialFolder.SendTo)}\{Dictionary.InstallerWindows_ContextMenuEncrypt}.lnk", "CryptoApp_CommandArgs_Encrypt");
            Windows_WriteShortcut($@"{Environment.GetFolderPath(Environment.SpecialFolder.SendTo)}\{Dictionary.InstallerWindows_ContextMenuDecrypt}.lnk", "CryptoApp_CommandArgs_Decrypt");
            Windows_WriteShortcut($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Privateer.lnk");
            var thread = new Thread(new ThreadStart(AnimateDownwards));
            thread.Start();
        }

        private void Btn_finish_OnClick(object? sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Windows_WriteShortcut(string path, string arguments = "")
        {
            string shortcutLocation = path;
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
            shortcut.IconLocation = txt_path.Text + @"/icon.ico";   // The icon of the shortcut
            shortcut.Arguments = arguments;
            shortcut.TargetPath = txt_path.Text + @"/CryptoGUIAvalonia.exe";                 // The path of the file that will launch when the shortcut is run
            shortcut.Save();                                    // Save the shortcut
        }
    }
}