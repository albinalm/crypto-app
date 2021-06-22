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
using Mono.Unix;

namespace CryptoInstallerLinux
{
    public class MainWindow : Window
    {
        private Dict Dictionary { get; set; }
        
        private bool isResizing = false;

        private Panel FirstPane;
        private Panel SecondPane;
        private Panel ThirdPane;
        
        private Image img_next;
        private Image img_fm;
        private Image img_icon;
        private Image img_icon2;
        private Image img_icon3;
        private Image img_example;
        
        private Label lbl_fm;
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
            border_fm = this.Get<Border>("border_fm");
            lbl_question = this.Get<Label>("lbl_question");
            lbl_welcome = this.Get<Label>("lbl_welcome");
            lbl_welcome2 = this.Get<Label>("lbl_welcome2");
            img_icon = this.Get<Image>("img_icon");
            img_icon2 = this.Get<Image>("img_icon2");
            img_icon3 = this.Get<Image>("img_icon3");
            img_fm = this.Get<Image>("img_kde");
            img_next = this.Get<Image>("img_next");
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
            img_fm.Source = ReadEmbeddedResourceImage("KDE_logo.png");
            img_next.Source = ReadEmbeddedResourceImage("next.png");
           
            btn_next = this.Get<Button>("btn_next");
            lbl_fm = this.Get<Label>("lbl_kde");
            txt_path = this.Get<TextBox>("txt_path");
            lbl_fm.Foreground = Brushes.DodgerBlue;
            txt_path.Text = $"/home/{Environment.UserName}/Privateer";
        }
        private Bitmap ReadEmbeddedResourceImage(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("CryptoInstallerLinux.Resources." + resourceName))
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
                        
                        this.Height++;
                        if (this.Height >= 900)
                        {
                            fullSize = true;
                            this.Height = 900;
                           
                           // FadeInLastElements();
                        }
                        
                    });
                    Thread.Sleep(1);
            } while (!fullSize);
            Thread.Sleep(500);
            Console.WriteLine("Fading!" + fullSize);
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
                    img_example.Opacity += 0.001;
                    lbl_help.Opacity += 0.001;
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
                 
                    btn_finish.Opacity += 0.001;
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
            using(var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("CryptoInstallerLinux.InstallData.core.zip"))
            {
                using(var file = new FileStream(AppContext.BaseDirectory + @"/core.zip", FileMode.Create, FileAccess.Write))
                {
                    resource?.CopyTo(file);
                } 
            }

            if (kde)
            {
                using(var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("CryptoInstallerLinux.InstallData.dolphin.zip"))
                {
                    using(var file = new FileStream(AppContext.BaseDirectory + @"/dolphin.zip", FileMode.Create, FileAccess.Write))
                    {
                        resource?.CopyTo(file);
                    } 
                }  
            }
        var thread = new Thread(new ThreadStart(Extract));
            thread.IsBackground = true;
            thread.Start();
        }
        private void Img_next_OnPointerEnter(object? sender, PointerEventArgs e)
        {
            img_next.Height -= 3;
        }

        private void Img_next_OnPointerLeave(object? sender, PointerEventArgs e)
        {
            img_next.Height += 3;
        }

        private void Img_next_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            img_next.Height -= 3;
        }

        private void Img_next_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            img_next.Height += 3;
            if (kde)
            {
                setNautilus();
            }
            else
            {
                setKde();
            }

            kde = !kde;
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
                    lbl_welcome.Opacity += 0.001;
                    img_icon.Opacity += 0.001;
                    if (lbl_welcome.Opacity >= 1)
                    {
                        isFaded = true;
                        lbl_welcome.Opacity = 1;
                        img_icon.Opacity = 1;
                    }
                    
                });
                Thread.Sleep(1);
            } while (!isFaded);

            Thread.Sleep(1000);
            FadeInRest();
        }
        private void FadeInRest()
        {
            var isFaded = false;
            var hasFadedButton = false;
            do
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    border_fm.Opacity+=0.001;
                    lbl_fm.Opacity += 0.001;
                    img_next.Opacity += 0.001;
                    lbl_question.Opacity += 0.001;
                    if (border_fm.Opacity >= 1)
                    {
                        isFaded = true;
                        border_fm.Opacity = 1;
                        lbl_fm.Opacity = 1;
                        img_next.Opacity = 1;
                        lbl_question.Opacity = 1;
                    }

                });
                Thread.Sleep(1);
            } while (!isFaded);

            FadeInButton();
        }

        private void FadeInButton()
        {
            var isFaded = false;
            do
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    btn_next.Opacity += 0.001;
                    if (btn_next.Opacity >= 1)
                    {
                        isFaded = true;
                        btn_next.Opacity = 1;
                    }
                    
                });
                Thread.Sleep(1);
            } while (!isFaded);
        }
        private void setNautilus()
        {
            img_fm.Margin = new Thickness(7, 0, 0, 0);
            img_fm.Source = ReadEmbeddedResourceImage("gnome.png");
            lbl_fm.Content = "GNOME Nautilus";
            lbl_fm.FontWeight = FontWeight.ExtraLight;
            lbl_fm.Foreground = Brushes.MediumBlue;
            
        }

        private void setKde()
        {
            img_fm.Margin = new Thickness(5, 0, 0, 0);
            img_fm.Source = ReadEmbeddedResourceImage("KDE_logo.png");
            lbl_fm.Content = "KDE Dolphin";
            lbl_fm.FontWeight = FontWeight.ExtraBold;
            lbl_fm.Foreground = Brushes.DodgerBlue;
      
        }

        private void InitializeTranslation()
        {
            var language = System.Globalization.CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName;
            var engine = new TranslationEngine();
            
            Dictionary = engine.InitializeLanguage(TranslationEngine.Languages.Contains(language) ? language : "eng");
            lbl_welcome.Content = Dictionary.InstallerLinux_WelcomeTo;
            lbl_welcome2.Content = Dictionary.InstallerLinux_WelcomeTo;
            lbl_question.Content = Dictionary.InstallerLinux_WhatFMQuestion;
            btn_next.Content = Dictionary.InstallerLinux_Next;
            btn_back.Content = Dictionary.InstallerLinux_Back;
            lbl_path.Content = Dictionary.InstallerLinux_WhereToInstall;
            btn_install.Content = Dictionary.InstallerLinux_Install;
            Title = Dictionary.InstallerLinux_Title;
            lbl_help.Content = Dictionary.InstallerLinux_UseDescription;
            lbl_status.Content = Dictionary.InstallerLinux_Installing;
            btn_finish.Content = Dictionary.InstallerLinux_Finish;
            chk_start.Content = Dictionary.InstallerLinux_OpenPrivateer;
          
            lbl_finished.Content = Dictionary.InstallerLinux_FinishedInstalling;
            if(TranslationEngine.Languages.Contains(language))
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
            txt_path.Text = $"{result}/Privateer";
        }

        private void Btn_install_OnClick(object? sender, RoutedEventArgs e)
        {
            Title = Dictionary.InstallerLinux_TitleInstalling;
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
 
            using (var zip = ZipFile.Read(AppContext.BaseDirectory + @"/core.zip"))
            {
                zip.ExtractAll(txt_path.Text);
            }
            
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                pb_progress.Value = 8;
            });
            if (kde)
            {
                using (var zip = ZipFile.Read(AppContext.BaseDirectory + @"/dolphin.zip"))
                {
                    zip.ExtractAll(txt_path.Text);
                }  
                File.Delete(AppContext.BaseDirectory + @"/dolphin.zip");
            }
            File.Delete(AppContext.BaseDirectory + @"/core.zip");
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                pb_progress.Value = 10;
                SecondPane.IsVisible = false;
                ThirdPane.IsVisible = true;
                Title = Dictionary.InstallerLinux_TitleFinish;
            });
           if(kde)
               Dolphin_SetContextMenuItem();
            var thread = new Thread(new ThreadStart(AnimateDownwards));
            thread.Start();
        }

        private void Btn_finish_OnClick(object? sender, RoutedEventArgs e)
        {
            Exec(txt_path.Text + @"/CryptoGUIAvalonia");
            if (kde)
            {
                Exec(txt_path.Text + @"/contextmenubinaries/CryptoDolphinLauncher");
                Exec(txt_path.Text + @"/contextmenubinaries/writer/CryptoDolphinFileWriter");
            }
            Environment.Exit(0);

        }
        private void Exec(string cmd)
        {
            var unixFileInfo = new Mono.Unix.UnixFileInfo(cmd);
            // set file permission to 644
          
            unixFileInfo.FileAccessPermissions =
                FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite
                                               | FileAccessPermissions.GroupRead
                                               | FileAccessPermissions.OtherRead 
                                               | FileAccessPermissions.UserExecute;
        }
        private void Dolphin_SetContextMenuItem()
        {
           
            var writer = new StreamWriter($"/home/{Environment.UserName}/.local/share/kservices5/ServiceMenus/Privateer.desktop");
            writer.WriteLine("[Desktop Entry]");
            writer.WriteLine("Type=Service");
            writer.WriteLine("ServiceTypes=KonqPopupMenu/Plugin,inode/directory,all/allfiles");
            writer.WriteLine("Actions=encrypt;decrypt;manage;");
            writer.WriteLine("X-KDE-Priority=TopLevel");
            writer.WriteLine("X-KDE-Submenu=Privateer");
            writer.WriteLine("Icon=" + txt_path.Text + @"/Resources/icon.png");
            writer.WriteLine("");
            writer.WriteLine("[Desktop Action encrypt]");
            writer.WriteLine("");
            writer.WriteLine("Exec=\"" + txt_path.Text + "/contextmenubinaries/writer/CryptoDolphinFileWriter\" CryptoApp_CommandArgs_Encrypt");
            writer.WriteLine("Name=" + Dictionary.InstallerLinux_ContextMenuEncrypt);
            writer.WriteLine("Icon=" + txt_path.Text + @"/Resources/icon.png");
            writer.WriteLine("");
            writer.WriteLine("[Desktop Action decrypt]");
            writer.WriteLine("");
            writer.WriteLine("Exec=\"" + txt_path.Text + "/contextmenubinaries/writer/CryptoDolphinFileWriter\" CryptoApp_CommandArgs_Decrypt");
            writer.WriteLine("Name=" + Dictionary.InstallerLinux_ContextMenuDecrypt);
            writer.WriteLine("Icon=" + txt_path.Text + @"/Resources/icon.png");
            writer.WriteLine("");
            writer.WriteLine("[Desktop Action manage]");
            writer.WriteLine("");
            writer.WriteLine("Exec=\"" + txt_path.Text + "/contextmenubinaries/writer/CryptoDolphinFileWriter\" CryptoApp_CommandArgs_ManageKeys");
            writer.WriteLine("Name=" + Dictionary.InstallerLinux_ContextMenuManageKeys);
            writer.WriteLine("Icon=" + txt_path.Text + @"/Resources/icon.png");
            writer.Flush();
            writer.Close();
            Exec($"/home/{Environment.UserName}/.local/share/kservices5/ServiceMenus/Privateer.desktop");
         //
        }
        
    }
}