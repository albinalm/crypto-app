using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryptoAPI.ORM;
using CryptoGUI.DataModel;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;
using CryptoTranslation;

namespace CryptoGUIAvalonia
{
    public class EncryptorArray : Window
    {
        private Dict Dictionary;
        private int FilesEncrypted;
        private int SpeedCalculator_Increment;
        private string CurrentFileSource = "";
        private bool CalculateSpeed = true;
        private bool UpdateGui = true;
        private Label lbl_title { get; set; }
        private Label lbl_percentage { get; set; }
        private Label lbl_destination_path { get; set; }
        private Label lbl_speed { get; set; }
        private ProgressBar pb_current { get; set; }
        private ProgressBar pb_total { get; set; }
        private Label lbl_destination { get; set; }
        private Label lbl_currentFile { get; set; }
        private TextBlock txtblock_destination_path { get; set; }
        private TextBlock txtblock_currentFile { get; set; }

        private Thread _TrackProgress { get; set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public EncryptorArray()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var logoImage = this.Get<Image>("img_icon");
            //logoImage.Source = "/Resources/logo02.png";
            logoImage.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/logo02.png");
            Icon = new WindowIcon(new Bitmap(Environment.CurrentDirectory + "/Resources/icon.png"));
            InitializeTranslation();
            this.Closing += OnClosing;

            #region UI Declarations

            lbl_destination_path = this.Get<Label>("lbl_destination_path");
            lbl_percentage = this.Get<Label>("lbl_percentage");
            lbl_title = this.Get<Label>("lbl_title");
            lbl_speed = this.Get<Label>("lbl_speed");
            lbl_destination = this.Get<Label>("lbl_destination");
            pb_current = this.Get<ProgressBar>("pb_current");
            pb_total = this.Get<ProgressBar>("pb_total");
            lbl_currentFile = this.Get<Label>("lbl_currentFile");
            txtblock_destination_path = this.Get<TextBlock>("txtblock_destination_path");
            txtblock_currentFile = this.Get<TextBlock>("txtblock_currentFile");

            #endregion UI Declarations

            _TrackProgress = new Thread(new ThreadStart(TrackProgress)) { IsBackground = true };
            Startup();
        }

        private void InitializeTranslation()
        {
            var language = System.Globalization.CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName;
            var engine = new TranslationEngine();
            Dictionary = engine.InitializeLanguage(TranslationEngine.Languages.Contains(language) ? language : "eng");
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ExecuteAsync_Worker()
        {
            var thread = new Thread(new ThreadStart(Worker)) { IsBackground = true };
            thread.Start();
        }

        private void ExecuteAsync_TrackProgress()
        {
            _TrackProgress = new Thread(new ThreadStart(TrackProgress)) { IsBackground = true };
            _TrackProgress.Start();
        }

        private void Worker()
        {
            Thread.Sleep(1000);
            var takenSources = new List<string>();
            foreach (var source in EncryptionData.Sources)
            {
                CurrentFileSource = source;
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    pb_total.Value = FilesEncrypted;
                    //     lbl_percentage.Content = Math.Round(pb_current.Value / pb_current.Maximum * 100, 0) + "%";
                });
                SpeedCalculator_Increment = 0;
                //Check if file is inside a formatted subdirectory
                if (EncryptionData.RootSubdirectories.Count > 0)
                {
                    foreach (var subdir in EncryptionData.RootSubdirectories)
                    {
                        if (source.StartsWith(subdir + Path.DirectorySeparatorChar))
                        {
                            //C:/Users/Albin/Desktop/Ny mapp/Ny mapp2/Letgooo.exe
                            var safeName = new DirectoryInfo(subdir).Name;
                            var restPath = CurrentFileSource.Remove(0, subdir.Length);
                            if (!Directory.Exists(Path.GetDirectoryName($"{subdir}_encrypted{restPath}")))
                                Directory.CreateDirectory(Path.GetDirectoryName($"{subdir}_encrypted{restPath}"));
                            EncryptionData.DestinationFileName = $"{subdir}_encrypted{restPath}";

                            Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                Title = @$"{Dictionary.EncryptionArray_Title} " + subdir;
                                lbl_title.Content = $"{Dictionary.EncryptionArray_Encrypting}..";
                                txtblock_destination_path.Text = EncryptionData.DestinationFileName;
                                txtblock_currentFile.Text = EncryptionData.DestinationFileName;
                                pb_current.Value = 0;
                                lbl_percentage.Content = $"{Dictionary.EncryptionArray_EncryptingFile} {FilesEncrypted} {Dictionary.EncryptionArray_EncryptingFileOf} {EncryptionData.Sources.Count}";
                                lbl_speed.Content = $"{Dictionary.EncryptionArray_Speed}: --";
                            });
                            if (new FileInfo(source).Length > 1000000) //10000000
                            {
                                CalculateSpeed = true;
                                ExecuteAsync_SpeedCalculator();
                                if (!_TrackProgress.IsAlive)
                                {
                                    ExecuteAsync_TrackProgress();
                                }
                            }
                            Cryptography.Encryption.EncryptFile(source, EncryptionData.DestinationFileName, 1024);
                            takenSources.Add(source);
                            FilesEncrypted++;
                        }
                    }
                    if (!takenSources.Contains(source))
                    {
                        var safeFileName = Path.GetFileNameWithoutExtension(source);
                        var safeDirName = Path.GetDirectoryName(source);
                        var fileCount = new DirectoryInfo(safeDirName).GetFiles().Count(finf => finf.Name.StartsWith(Path.GetFileNameWithoutExtension(source) + "_encrypted"));
                        if (fileCount == 0)
                            EncryptionData.DestinationFileName = safeDirName + "/" + safeFileName + "_encrypted" +
                                                                 Path.GetExtension(source);
                        else
                            EncryptionData.DestinationFileName = safeDirName + "/" + safeFileName + "_encrypted_" +
                                                                 fileCount + Path.GetExtension(source);

                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            txtblock_currentFile.Text = safeFileName + Path.GetExtension(source);
                            pb_current.Value = 0;
                            Title = @$"{Dictionary.EncryptionArray_Title} " + safeDirName;
                            lbl_title.Content = $"{Dictionary.EncryptionArray_Encrypting}..";
                            txtblock_destination_path.Text = $"{safeDirName}{Path.DirectorySeparatorChar}{safeFileName}{Path.GetExtension(source)}";
                            lbl_percentage.Content = $"{Dictionary.EncryptionArray_EncryptingFile} {FilesEncrypted} {Dictionary.EncryptionArray_EncryptingFileOf} {EncryptionData.Sources.Count}";
                            lbl_speed.Content = $"{Dictionary.EncryptionArray_Speed}: --";
                        });
                        if (new FileInfo(source).Length > 1000000) //10000000
                        {
                            CalculateSpeed = true;
                            ExecuteAsync_SpeedCalculator();
                            if (!_TrackProgress.IsAlive)
                            {
                                ExecuteAsync_TrackProgress();
                            }
                        }
                        Cryptography.Encryption.EncryptFile(source, EncryptionData.DestinationFileName, 1024);
                        FilesEncrypted++;
                    }
                }
                else
                {
                    var safeFileName = Path.GetFileNameWithoutExtension(source);
                    var safeDirName = Path.GetDirectoryName(source);
                    var fileCount = new DirectoryInfo(safeDirName).GetFiles().Count(finf => finf.Name.StartsWith(Path.GetFileNameWithoutExtension(source) + "_encrypted"));
                    if (fileCount == 0)
                        EncryptionData.DestinationFileName = safeDirName + "/" + safeFileName + "_encrypted" +
                                                             Path.GetExtension(source);
                    else
                        EncryptionData.DestinationFileName = safeDirName + "/" + safeFileName + "_encrypted_" +
                                                             fileCount + Path.GetExtension(source);

                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        Title = @$"{Dictionary.EncryptionArray_Title} " + safeDirName;
                        lbl_title.Content = $"{Dictionary.EncryptionArray_Encrypting}..";
                        txtblock_destination_path.Text = $"{safeDirName}{Path.DirectorySeparatorChar}{safeFileName}{Path.GetExtension(source)}";
                        txtblock_currentFile.Text = safeFileName + Path.GetExtension(source);
                        pb_current.Value = 0;
                        lbl_percentage.Content = $"{Dictionary.EncryptionArray_EncryptingFile} {FilesEncrypted} {Dictionary.EncryptionArray_EncryptingFileOf} {EncryptionData.Sources.Count}";
                        lbl_speed.Content = $"{Dictionary.EncryptionArray_Speed}: --";
                    });
                    if (new FileInfo(source).Length > 1000000) //10000000
                    {
                        CalculateSpeed = true;
                        ExecuteAsync_SpeedCalculator();
                        if (!_TrackProgress.IsAlive)
                        {
                            ExecuteAsync_TrackProgress();
                        }
                    }
                    Cryptography.Encryption.EncryptFile(source, EncryptionData.DestinationFileName, 1024);
                    FilesEncrypted++;
                }
            }
            Environment.Exit(0);
        }

        private void TrackProgress()
        {
            // Thread.Sleep(1000);
            try
            {
                var fileLength = Math.Round((double)new FileInfo(CurrentFileSource).Length / 1048576, 0);
                var runloop = true;
                Dispatcher.UIThread.InvokeAsync(() => { pb_current.Maximum = fileLength; });

                while (runloop)
                    if (File.Exists(EncryptionData.DestinationFileName))
                    {
                        Thread.Sleep(10);
                        FileInfo finf = new(EncryptionData.DestinationFileName);
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            lbl_percentage.Content = $"{Dictionary.EncryptionArray_EncryptingFile} {FilesEncrypted} {Dictionary.EncryptionArray_EncryptingFileOf} {EncryptionData.Sources.Count} {Math.Round(pb_current.Value / pb_current.Maximum * 100, 0)}%";
                            pb_current.Value = Math.Round((double)finf.Length / 1048576, 0);
                            if (pb_current.Value != pb_current.Maximum) return;

                            CalculateSpeed = false;
                            lbl_speed.Content = $"{Dictionary.EncryptionArray_Speed}: --";
                            lbl_percentage.Content = $"{Dictionary.EncryptionArray_EncryptingFile} {FilesEncrypted} {Dictionary.EncryptionArray_EncryptingFileOf} {EncryptionData.Sources.Count} {Dictionary.EncryptionArray_Finalizing}";
                            lbl_title.Content = $"{Dictionary.EncryptionArray_FinishingUp}...";
                            runloop = false;
                        });
                    }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(this, ex.ToString(), "An error has occurred", MessageBox.MessageBoxButtons.Ok);
                File.WriteAllText("/home/albin/RiderProjects/crypto-app/Source/CryptoGUIAvalonia/bin/Any CPU/Debug/net5.0/linux-x64/log.txt", ex.ToString());
            }
        }

        private void ExecuteAsync_SpeedCalculator()
        {
            var thread = new Thread(new ThreadStart(SpeedCalculator)) { IsBackground = true };
            thread.Start();
        }

        private void SpeedCalculator()
        {
            do
            {
                SpeedCalculator_Increment++;
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var sizeDiff =
                        pb_current.Value / SpeedCalculator_Increment;
                    lbl_speed.Content = $"{Dictionary.EncryptionArray_Speed}: {Math.Round(sizeDiff * 5, 1)} MB/s";
                });

                Thread.Sleep(200);
            } while (CalculateSpeed);
        }

        private void ExecuteAsync_GuiUpdater()
        {
            var thread = new Thread(new ThreadStart(GuiUpdater)) { IsBackground = true };
            thread.Start();
        }

        private void GuiUpdater()
        {
            do
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (lbl_title.Content == Dictionary.EncryptionArray_Encrypting)
                        lbl_title.Content = $"{Dictionary.EncryptionArray_Encrypting}.";

                    if (lbl_title.Content == $"{Dictionary.EncryptionArray_Encrypting}.")
                        lbl_title.Content = $"{Dictionary.EncryptionArray_Encrypting}..";

                    if (lbl_title.Content == $"{Dictionary.EncryptionArray_Encrypting}..")
                        lbl_title.Content = $"{Dictionary.EncryptionArray_Encrypting}...";

                    if (lbl_title.Content == $"{Dictionary.EncryptionArray_Encrypting}...")
                        lbl_title.Content = Dictionary.EncryptionArray_Encrypting;

                    if (lbl_title.Content == Dictionary.EncryptionArray_FinishingUp)
                        lbl_title.Content = $"{Dictionary.EncryptionArray_FinishingUp}.";

                    if (lbl_title.Content == $"{Dictionary.EncryptionArray_FinishingUp}.")
                        lbl_title.Content = $"{Dictionary.EncryptionArray_FinishingUp}..";

                    if (lbl_title.Content == $"{Dictionary.EncryptionArray_FinishingUp}..")
                        lbl_title.Content = $"{Dictionary.EncryptionArray_FinishingUp}...";

                    if (lbl_title.Content == $"{Dictionary.EncryptionArray_FinishingUp}...")
                        lbl_title.Content = Dictionary.EncryptionArray_FinishingUp;
                });
                Thread.Sleep(200);
            } while (UpdateGui);
        }

        private void Startup()
        {
            FilesEncrypted = 1; //Set it as one so we normalize "Count"
            UpdateGui = true;
            ExecuteAsync_GuiUpdater();
            lbl_destination.Content = $"{Dictionary.EncryptionArray_Destination}:";
            Title = $"{Dictionary.EncryptionArray_Encrypting}...";
            pb_total.Maximum = EncryptionData.Sources.Count;
            ExecuteAsync_Worker();
        }
    }
}