using System;
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

namespace CryptoInstallerLinux
{
    public class MainWindow : Window
    {
        private Dict Dictionary { get; set; }
        
        private bool isResizing = false;

        private Panel FirstPane;
        private Panel SecondPane;
        
        
        private Image img_next;
        private Image img_fm;
        private Image img_icon;
        private Image img_icon2;
        
        private Label lbl_fm;
        private Label lbl_question;
        private Label lbl_welcome;
        private Label lbl_welcome2;
        private Label lbl_path;
        
        private Border border_fm;

        private Button btn_next;
        private Button btn_back;
        private Button btn_install;
        
        
        private TextBox txt_path;
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
            img_fm = this.Get<Image>("img_kde");
            img_next = this.Get<Image>("img_next");
            FirstPane = this.Get<Panel>("FirstPane");
            SecondPane = this.Get<Panel>("SecondPane");
            lbl_path = this.Get<Label>("lbl_path");
            btn_back = this.Get<Button>("btn_back");
            btn_install = this.Get<Button>("btn_install");
            img_icon.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/logo01.png");
            img_icon2.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/logo01.png");
            img_fm.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/KDE_logo.png");
            img_next.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/next.png");
            btn_next = this.Get<Button>("btn_next");
            lbl_fm = this.Get<Label>("lbl_kde");
            txt_path = this.Get<TextBox>("txt_path");
            lbl_fm.Foreground = Brushes.DodgerBlue;
            txt_path.Text = $"/home/{Environment.UserName}/Privateer";
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
            img_fm.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/gnome.png");
            lbl_fm.Content = "GNOME Nautilus";
            lbl_fm.FontWeight = FontWeight.ExtraLight;
            lbl_fm.Foreground = Brushes.MediumBlue;
            
        }

        private void setKde()
        {
            img_fm.Margin = new Thickness(5, 0, 0, 0);
            img_fm.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/KDE_logo.png");
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
    }
}