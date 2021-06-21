using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CryptoAPI.ORM;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;
using Ionic.Zip;
using Ionic.Zlib;
using CryptoTranslation;

namespace CryptoGUIAvalonia
{
    public class Index : Window
    {
        private Dict Dictionary;
        private Label lbl_validation;

        private Label lbl_details;
        private Label lbl_enterkey;
        private Label lbl_keyfound;
        private Label lbl_keydate;
        private Label lbl_keylocation;
        private Label lbl_checkingforupdates;
        private Label lbl_keydetails;
        private TextBox txt_pass;
        private Button btn_newkey;
        private Button btn_loadkey;
        private Button btn_validatekey;
        private Button btn_update;
        private Image btn_settings;
        private Thickness txt_passMargin;
        private string Mode = "";
        private string LoadKey_FileName = "";
        private string ValidateKey_FileName = "";
        private bool isResizing = false;
        private bool isMaxMode = false;
        private string UpdateLabelTextMode = "Checking";
        private bool UpdateLabelGUIRefresh = false;
        private int MinHeight = 450;
        private int MaxHeight = 485;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public Index()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var logoImage = this.Get<Image>("img_icon");
            //logoImage.Source = "/Resources/logo02.png";
            logoImage.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/logo01.png");
            this.Get<TextBox>("txt_pass").GotFocus += txt_pass_gotFocus;
            this.LayoutUpdated += OnLayoutUpdated;
            this.Closing += OnClosing;
            Icon = new WindowIcon(new Bitmap(Environment.CurrentDirectory + "/Resources/icon.png"));
            if (File.Exists(Environment.CurrentDirectory + @"/update.zip"))
                File.Delete(Environment.CurrentDirectory + @"/update.zip");
            lbl_validation = this.Get<Label>("lbl_validation");
            lbl_details = this.Get<Label>("lbl_details");
            lbl_enterkey = this.Get<Label>("lbl_enterkey");
            lbl_keyfound = this.Get<Label>("lbl_keyfound");
            lbl_keydate = this.Get<Label>("lbl_keydate");
            lbl_keylocation = this.Get<Label>("lbl_keylocation");
            lbl_keydetails = this.Get<Label>("lbl_keydetails");
            txt_pass = this.Get<TextBox>("txt_pass");
            btn_newkey = this.Get<Button>("btn_newkey");
            btn_loadkey = this.Get<Button>("btn_loadkey");
            btn_validatekey = this.Get<Button>("btn_validatekey");
            btn_settings = this.Get<Image>("btn_settings");
            btn_settings.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/icn_settings.png");
            btn_settings.PointerEnter += Btn_settingsOnPointerEnter;
            btn_settings.PointerLeave += Btn_settingsOnPointerLeave;
            btn_settings.PointerReleased += Btn_settingsOnPointerReleased;

            lbl_checkingforupdates = this.Get<Label>("lbl_checkingforupdates");
            btn_update = this.Get<Button>("btn_update");
            txt_passMargin = txt_pass.Margin;

            this.PointerPressed += OnPointerPressed;
            Height = MaxHeight;
            lbl_details.LayoutUpdated += Lbl_details_LayoutUpdated;
            lbl_enterkey.LayoutUpdated += Lbl_enterkey_LayoutUpdated;
            lbl_checkingforupdates.LayoutUpdated += Lbl_checkingforupdates_LayoutUpdated;
            btn_update.Click += Btn_update_Click;
            InitializeTranslation();
            UpdateUI();
        }

        private void Btn_update_Click(object? sender, RoutedEventArgs e)
        {
            btn_loadkey.IsEnabled = false;
            btn_newkey.IsEnabled = false;
            btn_validatekey.IsEnabled = false;
            btn_update.IsVisible = false;
            lbl_keydetails.Foreground = Brush.Parse("Gray");
            lbl_keydate.Foreground = Brush.Parse("Gray");
            lbl_keylocation.Foreground = Brush.Parse("Gray");
            lbl_keyfound.Foreground = Brush.Parse("Gray");
            lbl_validation.Foreground = Brush.Parse("Gray");
            lbl_checkingforupdates.Content = Dictionary.Index_DownloadingUpdate;
            ExecuteAsync_DownloadingGUIUpdate();
            ExecuteAsync_UpdateApp();
        }

        private void ExecuteAsync_UpdateApp()
        {
            var thread = new Thread(new ThreadStart(UpdateApp));
            thread.IsBackground = true;
            thread.Start();
        }

        private void ExecuteAsync_DownloadingGUIUpdate()
        {
            var thread = new Thread(new ThreadStart(DownloadingGUIUpdate));
            thread.IsBackground = true;
            thread.Start();
        }

        private void InitializeTranslation()
        {
            var language = System.Globalization.CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName;
            var engine = new TranslationEngine();
            Dictionary = engine.InitializeLanguage(TranslationEngine.Languages.Contains(language) ? language : "eng");
            btn_loadkey.Content = Dictionary.Index_LoadKeyButton;
            btn_newkey.Content = Dictionary.Index_NewKeyButton;
            btn_update.Content = Dictionary.Index_UpdateButton;
            btn_validatekey.Content = Dictionary.Index_ValidateKeyButton;
            lbl_keydetails.Content = Dictionary.Index_KeyDetails;
            lbl_validation.Content = Dictionary.Index_NoValidation;
            lbl_enterkey.Content = Dictionary.Index_EnterToAccept;
            this.Title = Dictionary.Index_Title;
        }

        private void UpdateApp()
        {
            if (Environment.OSVersion.ToString().Contains("Windows"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://github.com/albinalm/crypto-app/raw/main/Updating/archive_windows.zip", Environment.CurrentDirectory + @"/update.zip");
                }
                Process.Start(Environment.CurrentDirectory + @"/CryptoUpdater.exe");
                Environment.Exit(0);
            }
            else if (Environment.OSVersion.ToString().Contains("Unix"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://github.com/albinalm/crypto-app/raw/main/Updating/archive_linux.zip", Environment.CurrentDirectory + @"/update.zip");
                }
                Process.Start(Environment.CurrentDirectory + @"/CryptoUpdater");
                Environment.Exit(0);
            }
        }

        private void DownloadingGUIUpdate()
        {
            double downloadLength = 0;
            while (true)
            {
                if (File.Exists(Environment.CurrentDirectory + @"/update.zip"))
                    downloadLength = new FileInfo(Environment.CurrentDirectory + @"/update.zip").Length;
            
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if(lbl_checkingforupdates.Content.ToString() == Dictionary.Index_DownloadingUpdate)
                        lbl_checkingforupdates.Content = $"{Dictionary.Index_DownloadingUpdate}    | {Math.Round(downloadLength / 1048576, 2)} MB";
                    else if (lbl_checkingforupdates.Content.ToString().Split("|")[0] == $"{Dictionary.Index_DownloadingUpdate}    ")
                        lbl_checkingforupdates.Content = $"{Dictionary.Index_DownloadingUpdate}.   | {Math.Round(downloadLength / 1048576, 2)} MB";
                    else if (lbl_checkingforupdates.Content.ToString().Split("|")[0] == $"{Dictionary.Index_DownloadingUpdate}.   ")
                        lbl_checkingforupdates.Content = $"{Dictionary.Index_DownloadingUpdate}..  | {Math.Round(downloadLength / 1048576, 2)} MB";
                    else if (lbl_checkingforupdates.Content.ToString().Split("|")[0] == $"{Dictionary.Index_DownloadingUpdate}..  ")
                        lbl_checkingforupdates.Content = $"{Dictionary.Index_DownloadingUpdate}... | {Math.Round(downloadLength / 1048576, 2)} MB";
                    else if (lbl_checkingforupdates.Content.ToString().Split("|")[0] == $"{Dictionary.Index_DownloadingUpdate}... ")
                        lbl_checkingforupdates.Content = $"{Dictionary.Index_DownloadingUpdate}    | {Math.Round(downloadLength / 1048576, 2)} MB";
                });
                Thread.Sleep(200);
            }
        }

        private void Lbl_checkingforupdates_LayoutUpdated(object? sender, EventArgs e)
        {
            double marginLeft = lbl_checkingforupdates.Bounds.Width + 10;

            btn_update.Margin = Thickness.Parse($"{ marginLeft},0,0,3");
            //this.Title = btn_update.Margin.ToString();
        }

        private void Lbl_enterkey_LayoutUpdated(object? sender, EventArgs e)
        {
            double marginRight = (Width - txt_pass.Width) - lbl_enterkey.Bounds.Width + 100;
            double marginBottom = (txt_pass.Height) + txt_passMargin.Bottom - txt_pass.Bounds.Height;
            //  lbl_enterkeyMargin = new Thickness(0, 0, 273, 37);

            lbl_enterkey.Margin = Thickness.Parse($"0,0,{marginRight},{marginBottom}");
        }

        private void Lbl_details_LayoutUpdated(object? sender, EventArgs e)
        {
            double marginRight = (Width - txt_pass.Width) - lbl_details.Bounds.Width + 100;
            double marginBottom = (txt_pass.Height) + lbl_details.Bounds.Height + txt_passMargin.Bottom;
            lbl_details.Margin = Thickness.Parse($"0,0,{marginRight},{marginBottom}");
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            btn_settings.Height = 57;
            btn_settings.Width = 57;
        }

        private void Btn_settingsOnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            btn_settings.Height = 60;
            btn_settings.Width = 60;
            btn_settings.IsEnabled = false;
            ShowSettingsDialogue();
        }

        private async Task ShowSettingsDialogue()
        {
            var dlg = new SettingsDialogue();
            await dlg.ShowDialog(this);
            btn_settings.IsEnabled = true;
        }

        private void Btn_settingsOnPointerLeave(object? sender, PointerEventArgs e)
        {
            btn_settings.Height = 64;
            btn_settings.Width = 64;
        }

        private void Btn_settingsOnPointerEnter(object? sender, PointerEventArgs e)
        {
            btn_settings.Height = 60;
            btn_settings.Width = 60;
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            if (!isResizing)
            {
                if (!isMaxMode)
                {
                    Height = MinHeight;
                    Width = 700;
                }
                else
                {
                    Height = MaxHeight;
                    Width = 700;
                }
            }
        }

        private void UpdateUI()
        {
            if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "config.ini"))
            {
                var keyReader = new StreamReader(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "config.ini");
                var keyPath = keyReader.ReadLine();
                keyReader.Close();
                lbl_keylocation.Content = $"{Dictionary.Index_KeyLocation} {keyPath}";
                lbl_keydate.Content = $"{Dictionary.Index_KeyDate} --";
                lbl_keylocation.Foreground = Brushes.Black;
                if (File.Exists(keyPath))
                {
                    lbl_keydate.Content = $"{Dictionary.Index_KeyDate} {new FileInfo(keyPath).CreationTime.ToString("g")}";
                    lbl_keyfound.Content = Dictionary.Index_KeyFound;
                    lbl_keyfound.Foreground = Brushes.Green;
                    ValidateKey_FileName = keyPath;
                }
                else
                {
                    lbl_keydate.Content = $"{Dictionary.Index_KeyDate} --";
                    lbl_keyfound.Content = Dictionary.Index_KeyNotFound;
                    lbl_keyfound.Foreground = Brushes.Red;
                    btn_validatekey.IsEnabled = false;
                }
            }
            else
            {
                lbl_keydate.Content = $"{Dictionary.Index_KeyDate} --";
                lbl_keylocation.Content = $"Expected location: {Dictionary.Index_NotSet}";
                lbl_keylocation.Foreground = Brushes.Red;
                lbl_keyfound.Content = Dictionary.Index_KeyNoExist;
                lbl_keyfound.Foreground = Brushes.Red;
                btn_validatekey.IsEnabled = false;
            }

            var executeUpdater = new Thread(new ThreadStart(CheckForUpdates));
            executeUpdater.Start();
        }

        private void CheckForUpdates()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                //   lbl_checkingforupdates.IsVisible = false;
                try
                {
                    using (var client = new WebClient())
                    {
                        var currentRevision = Assembly.GetEntryAssembly()?.GetName().Version;
                        client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                        var result = client.DownloadString("https://raw.githubusercontent.com/albinalm/crypto-app/main/Updating/version.ini");
                        if (!result.Contains("NoVersion"))
                        {
                            var liveVersion = Version.Parse(result);
                            if (currentRevision != liveVersion)
                            {
                                btn_update.IsVisible = true;
                                lbl_checkingforupdates.Content = Dictionary.Index_UpdateAvailable;
                                lbl_checkingforupdates.Foreground = Brush.Parse("Green");
                                MinHeight = 485;
                                MaxHeight = 550;
                                txt_passMargin = new Thickness(0, 0, 0, 70);
                            }
                            else
                            {
                                lbl_checkingforupdates.IsVisible = false;
                            }
                        }
                        else
                        {
                            lbl_checkingforupdates.IsVisible = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lbl_checkingforupdates.Content = Dictionary.Index_UpdateFailed;
                    MinHeight = 485;
                    MaxHeight = 550;
                    txt_passMargin = new Thickness(0, 0, 0, 70);
                    lbl_checkingforupdates.Foreground = Brush.Parse("Red");
                }
            });
        }

        private void btn_newKey_click(object sender, RoutedEventArgs e)
        {
            btn_loadkey.IsEnabled = true;
            btn_validatekey.IsEnabled = true;
            btn_newkey.IsEnabled = false;
            lbl_details.Content = $"{Dictionary.Index_NewKeyPasswordLabel}:";
            Mode = "NewKey";
            if (Height == MinHeight)
            {
                var thread = new Thread(new ThreadStart(AnimateDownwards));
                thread.Start();
            }
        }

        private void ValidateKey(string path, string password)
        {
            var zipFailed = false;
            using (ZipFile zip = ZipFile.Read(path))
            {
            
                zip.CompressionLevel = CompressionLevel.None;
                
                try
                {
                    zip.ExtractAll(Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
                catch
                {
                    zipFailed = true;
                    lbl_validation.Content = Dictionary.Index_ValidationFailed;
                    lbl_validation.Foreground = Brushes.Red;
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval");
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey");
                    if (Height == MaxHeight)
                    {
                        lbl_details.IsVisible = false;
                        lbl_enterkey.IsVisible = false;
                        txt_pass.IsVisible = false;
                        var thread = new Thread(new ThreadStart(AnimateUpwards));
                        thread.Start();
                    }
                    else
                    {
                    }
                }
            }

            if (!zipFailed)
            {
                if (File.ReadAllLines(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval")[0] ==
                    Cryptography.Encryption.HashPassword(password))
                {
                    lbl_validation.Content = Dictionary.Index_ValidationSuccess;
                    lbl_validation.Foreground = Brushes.Green;
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval");
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey");
                    var writer =
                        new StreamWriter(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "config.ini");

                    writer.WriteLine(path);
                    writer.Flush();
                    writer.Close();
                    var hashWriter =
                        new StreamWriter(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "credential");
                    hashWriter.WriteLine(Cryptography.Encryption.HashPassword(password));
                    hashWriter.Flush();
                    hashWriter.Close();
                    UpdateUI();
                    if (Height == MaxHeight)
                    {
                        lbl_details.IsVisible = false;
                        lbl_enterkey.IsVisible = false;
                        txt_pass.IsVisible = false;
                        var thread = new Thread(new ThreadStart(AnimateUpwards));
                        thread.Start();
                    }
                    else
                    {
                    }
                }
                else
                {
                    lbl_validation.Content = Dictionary.Index_ValidationFailed;
                    lbl_validation.Foreground = Brushes.Red;
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "conf.eval");
                    if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey"))
                        File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "data.ekey");
                    if (Height == MaxHeight)
                    {
                        lbl_details.IsVisible = false;
                        lbl_enterkey.IsVisible = false;
                        txt_pass.IsVisible = false;
                        var thread = new Thread(new ThreadStart(AnimateUpwards));
                        thread.Start();
                    }
                    else
                    {
                    }
                }
            }
        }

        private async Task CreateNewKey(string password)
        {
            var dlg = new SaveFileDialog();
            var filter = new FileDialogFilter
            {
                Name = Dictionary.General_FileDialogFilter
            };
            filter.Extensions.Add("epak");
            dlg.Filters.Add(filter);
            dlg.Title = Dictionary.General_SaveFileDialogTitle;
            var _dlg = await dlg.ShowAsync(this);
            var result = false;
            var fileRes = "";
            fileRes = _dlg;
            if (!fileRes.EndsWith(".epak"))
                fileRes += ".epak";
            File.WriteAllBytes($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}data.ekey",
                Cryptography.GenerateEncryptionKey(password));
            File.WriteAllText($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}conf.eval", Cryptography.Encryption.HashPassword(password));
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.None;
                zip.AddFile($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}data.ekey", "");
                zip.AddFile($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}conf.eval", "");
                zip.Save(fileRes);
            }
            File.Delete($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}data.ekey");
            File.Delete($"{Path.GetDirectoryName(fileRes)}{Path.DirectorySeparatorChar}conf.eval");
            lbl_details.IsVisible = false;
            lbl_enterkey.IsVisible = false;
            txt_pass.IsVisible = false;
            var thread = new Thread(new ThreadStart(AnimateUpwards));
            thread.Start();
            ValidateKey(fileRes, password);
        }

        private void AnimateDownwards()
        {
            isResizing = true;
            var fullSize = false;
            do
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (this.Height >= MaxHeight)
                        fullSize = true;
                    this.Height += 2;
                });

                Thread.Sleep(1);
            } while (!fullSize);
            Dispatcher.UIThread.Post(() =>
            {
                lbl_details.IsVisible = true;
                lbl_enterkey.IsVisible = true;
                txt_pass.IsVisible = true;
                txt_pass.Margin = txt_passMargin;
                Height = MaxHeight;
            });
            isMaxMode = true;
            isResizing = false;
        }

        private void AnimateUpwards()
        {
            isResizing = true;
            var fullSize = false;
            do
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (this.Height <= MinHeight)
                        fullSize = true;
                    this.Height -= 2;
                });

                Thread.Sleep(1);
            } while (!fullSize);
            Dispatcher.UIThread.Post(() =>
            {
                btn_loadkey.IsEnabled = true;
                Height = MinHeight;
                btn_newkey.IsEnabled = true;
                btn_validatekey.IsEnabled = true;
            });
            isMaxMode = false;
            isResizing = false;
        }

        private void txt_pass_gotFocus(object? sender, GotFocusEventArgs e)
        {
            BorderBrush = Brushes.Red;
        }

        private void Txt_pass_OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = false;
                if (Mode == "NewKey")
                {
                    CreateNewKey(txt_pass.Text);
                }
                else if (Mode == "LoadKey")
                {
                    ValidateKey(LoadKey_FileName, txt_pass.Text);
                }
                else if (Mode == "ValidateKey")
                {
                    ValidateKey(ValidateKey_FileName, txt_pass.Text);
                }
            }
        }

        private void Btn_loadkey_OnClick(object? sender, RoutedEventArgs e)
        {
            btn_loadkey.IsEnabled = false;
            btn_newkey.IsEnabled = true;
            btn_validatekey.IsEnabled = true;
            lbl_details.Content = Dictionary.Index_LoadKeyPasswordLabel;

            LoadKey();
        }

        private async Task LoadKey()
        {
            var dlg = new OpenFileDialog();
            var filter = new FileDialogFilter
            {
                Name = Dictionary.General_FileDialogFilter,
            };
            filter.Extensions.Add("epak");
            dlg.Filters.Add(filter);
            dlg.Title = Dictionary.General_OpenFileDialogTitle;
            var _dlg = dlg.ShowAsync(this);
            var result = false;
            var fileRes = "";
            foreach (var res in await _dlg)
            {
                if (res.EndsWith(".epak"))
                {
                    fileRes = res;
                    result = true;
                }
            }

            if (result)
            {
                LoadKey_FileName = fileRes;
                Mode = "LoadKey";
                var thread = new Thread(new ThreadStart(AnimateDownwards));
                thread.Start();
            }
        }

        private void Btn_validatekey_OnClick(object? sender, RoutedEventArgs e)
        {
            btn_loadkey.IsEnabled = true;
            btn_validatekey.IsEnabled = false;
            btn_newkey.IsEnabled = true;
            lbl_details.Content = Dictionary.Index_ValidateKeyPasswordLabel;

            if (Height == MinHeight)
            {
                var thread = new Thread(new ThreadStart(AnimateDownwards));
                thread.Start();
            }
            else
            {
            }
            Mode = "ValidateKey";
        }
    }
}