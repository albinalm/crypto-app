using System;
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

namespace CryptoGUIAvalonia
{
    public class EncryptorArray : Window
    {
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
            foreach (var source in EncryptionData.Sources)
            {
                CurrentFileSource = source;
                Dispatcher.UIThread.Post(() =>
                {
                    pb_total.Value = FilesEncrypted;
                    //     lbl_percentage.Content = Math.Round(pb_current.Value / pb_current.Maximum * 100, 0) + "%";
                });
                SpeedCalculator_Increment = 0;
                var safeFileName = Path.GetFileNameWithoutExtension(source);
                var safeDirName =
                    source.Remove(source.Length - (safeFileName.Length + Path.GetExtension(source).Length + 1),
                        safeFileName.Length + Path.GetExtension(source).Length + 1);
                var fileCount = new DirectoryInfo(safeDirName).GetFiles().Count(finf => finf.Name.StartsWith(Path.GetFileNameWithoutExtension(source) + "_encrypted"));
                if (fileCount == 0)
                    EncryptionData.DestinationFileName = safeDirName + "/" + safeFileName + "_encrypted" +
                                                         Path.GetExtension(source);
                else
                    EncryptionData.DestinationFileName = safeDirName + "/" + safeFileName + "_encrypted_" +
                                                         fileCount + Path.GetExtension(source);
                Dispatcher.UIThread.Post(() =>
                {
                    Title = @"Encrypting files to: " + safeDirName;
                    lbl_title.Content = "Encrypting..";
                    txtblock_destination_path.Text = $"{safeDirName}{Path.DirectorySeparatorChar}{safeFileName}{Path.GetExtension(source)}";
                });
                CalculateSpeed = true;
                ExecuteAsync_SpeedCalculator();
                Dispatcher.UIThread.Post(() => { txtblock_currentFile.Text = safeFileName + Path.GetExtension(source); });

                Dispatcher.UIThread.Post(() => { pb_current.Value = 0; });
                if (!_TrackProgress.IsAlive)
                {
                    ExecuteAsync_TrackProgress();
                }
                Cryptography.Encryption.EncryptFile(source, EncryptionData.DestinationFileName, 1024);
                FilesEncrypted++;
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
                Dispatcher.UIThread.Post(() => { pb_current.Maximum = fileLength; });

                while (runloop)
                    if (File.Exists(EncryptionData.DestinationFileName))
                    {
                        Thread.Sleep(10);
                        FileInfo finf = new(EncryptionData.DestinationFileName);
                        Dispatcher.UIThread.Post(() =>
                        {
                            lbl_percentage.Content = $"Encrypting file {FilesEncrypted} of {EncryptionData.Sources.Count} {Math.Round(pb_current.Value / pb_current.Maximum * 100, 0)}%";
                            pb_current.Value = Math.Round((double)finf.Length / 1048576, 0);
                            if (pb_current.Value != pb_current.Maximum) return;

                            CalculateSpeed = false;
                            lbl_speed.Content = "Speed: --";
                            lbl_percentage.Content = $"Encrypting file {FilesEncrypted} of {EncryptionData.Sources.Count} Finalizing...";
                            lbl_title.Content = "Finishing up...";
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
                Dispatcher.UIThread.Post(() =>
                {
                    var sizeDiff =
                        pb_current.Value / SpeedCalculator_Increment;
                    lbl_speed.Content = $"Speed: {Math.Round(sizeDiff * 5, 1)} MB/s";
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
                Dispatcher.UIThread.Post(() =>
                {
                    lbl_title.Content = lbl_title.Content.ToString() switch
                    {
                        "Encrypting" => "Encrypting.",
                        "Encrypting." => "Encrypting..",
                        "Encrypting.." => "Encrypting...",
                        "Encrypting..." => "Encrypting",
                        "Finishing up" => "Finishing up.",
                        "Finishing up." => "Finishing up..",
                        "Finishing up.." => "Finishing up...",
                        "Finishing up..." => "Finishing up",
                        _ => lbl_title.Content
                    };
                });
                Thread.Sleep(200);
            } while (UpdateGui);
        }

        private void Startup()
        {
            FilesEncrypted = 1; //Set it as one so we normalize "Count"
            UpdateGui = true;
            ExecuteAsync_GuiUpdater();
            lbl_destination.Content = "Destination:";
            Title = @"Encrypting...";
            pb_total.Maximum = EncryptionData.Sources.Count;
            ExecuteAsync_Worker();
        }
    }
}