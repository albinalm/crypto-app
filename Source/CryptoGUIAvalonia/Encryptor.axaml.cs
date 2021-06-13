using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public class Encryptor : Window
    {
        private Label lbl_destination_path { get; set; }
        private Label lbl_percentage { get; set; }
        private Label lbl_title { get; set; }
        private Label lbl_speed { get; set; }
        private Label lbl_source { get; set; }
        private Label lbl_source_path { get; set; }
        private Label lbl_destination { get; set; }
        private ProgressBar pb_progress { get; set; }
        private int SpeedCalculator_Increment { get; set; }
        private bool CalculateSpeed = true;
        private bool UpdateGui = true;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public Encryptor()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var logoImage = this.Get<Image>("img_icon");
            //logoImage.Source = "/Resources/logo02.png";
            logoImage.Source = new Bitmap(Environment.CurrentDirectory + "/Resources/logo02.png");
            Icon = new WindowIcon(new Bitmap(Environment.CurrentDirectory + "/Resources/icon.png"));
            this.Closing += OnClosing;
            #region UI Declarations

            lbl_destination_path = this.Get<Label>("lbl_destination_path");
            lbl_percentage = this.Get<Label>("lbl_percentage");
            lbl_title = this.Get<Label>("lbl_title");
            lbl_speed = this.Get<Label>("lbl_speed");
            lbl_source = this.Get<Label>("lbl_source");
            lbl_source_path = this.Get<Label>("lbl_source_path");
            lbl_destination = this.Get<Label>("lbl_destination");
            pb_progress = this.Get<ProgressBar>("pb_progress");

            #endregion UI Declarations

            Startup();
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
            var thread = new Thread(new ThreadStart(TrackProgress)) { IsBackground = true };
            thread.Start();
        }

        private void Worker()
        {
            Thread.Sleep(1000);
            try
            {
                var safeFileName = Path.GetFileNameWithoutExtension(EncryptionData.SourceFileName);
                var safeDirName = EncryptionData.SourceFileName.Remove(
                    EncryptionData.SourceFileName.Length - (safeFileName.Length +
                                                            Path.GetExtension(EncryptionData.SourceFileName).Length +
                                                            1),
                    safeFileName.Length + Path.GetExtension(EncryptionData.SourceFileName).Length + 1);
                var fileCount = new DirectoryInfo(safeDirName).GetFiles().Count(finf =>
                    finf.Name.StartsWith(Path.GetFileNameWithoutExtension(EncryptionData.SourceFileName) +
                                         "_encrypted"));

                if (fileCount == 0)
                    EncryptionData.DestinationFileName = safeDirName + "/" + safeFileName + "_encrypted" +
                                                         Path.GetExtension(EncryptionData.SourceFileName);
                else
                    EncryptionData.DestinationFileName = safeDirName + "/" + safeFileName + "_encrypted_" + fileCount +
                                                         Path.GetExtension(EncryptionData.SourceFileName);
                Dispatcher.UIThread.Post(() =>
                {
                    lbl_destination_path.Content =
                        EncryptionData.DestinationFileName.Replace("_", "__"); //avoid mnemonics
                });
                // ExecuteAsync_TrackProgress();
                CalculateSpeed = true;
                ExecuteAsync_SpeedCalculator();
                Cryptography.Encryption.EncryptFile(EncryptionData.SourceFileName, EncryptionData.DestinationFileName,
                    1024);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                //   MessageBox.Show(this, ex.ToString(), "An error has occurred", MessageBox.MessageBoxButtons.Ok);
                Console.WriteLine(ex.ToString());
            }
        }

        private void TrackProgress()
        {
            // Thread.Sleep(1000);
            try
            {
                var fileLength = Math.Round((double)new FileInfo(EncryptionData.SourceFileName).Length / 1048576, 0);
                var runloop = true;
                Dispatcher.UIThread.Post(() =>
                {
                    pb_progress.Maximum = fileLength;
                });
                while (runloop)
                    if (File.Exists(EncryptionData.DestinationFileName))
                    {
                        Thread.Sleep(10);
                        FileInfo finf = new(EncryptionData.DestinationFileName);
                        Dispatcher.UIThread.Post(() =>
                        {
                            //Thread.Sleep(200);
                            pb_progress.Value = Math.Round((double)finf.Length / 1048576, 0);
                            lbl_percentage.Content = Math.Round(pb_progress.Value / pb_progress.Maximum * 100, 0) + "%";
                            if (pb_progress.Value != pb_progress.Maximum) return;
                            CalculateSpeed = false;
                            lbl_speed.Content = "Speed: --";
                            lbl_percentage.Content = "Finalizing file...";
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
                    var sizeDiff = pb_progress.Value / SpeedCalculator_Increment;
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
            UpdateGui = true;
            ExecuteAsync_GuiUpdater();
            lbl_source.Content = "Source:";
            lbl_source_path.Content = EncryptionData.SourceFileName;
            lbl_destination.Content = "Destination:";
            Title = @"Encrypting: " + EncryptionData.SourceFileName;

            ExecuteAsync_TrackProgress();
            ExecuteAsync_Worker();
        }
    }
}