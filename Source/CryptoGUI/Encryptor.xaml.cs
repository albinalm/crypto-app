using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using CryptoAPI.ORM;
using CryptoGUI.DataModel;

namespace CryptoGUI
{
    /// <summary>
    ///     Interaction logic for Encryptor.xaml
    /// </summary>
    public partial class Encryptor : Window
    {
        private readonly BackgroundWorker ProgressUpdater;
        private readonly Timer Speed_LabelUpdater;
        private readonly Timer Title_LabelUpdater;

        private readonly BackgroundWorker workHorse;
        private int SpeedCalculator_Increment;

        public Encryptor()
        {
            InitializeComponent();
            Loaded += Encryptor_Loaded;
            Closing += Encryptor_Closing;

            #region Declaration and Event assigns

            Title_LabelUpdater = new Timer
            {
                Interval = 500
            };
            Title_LabelUpdater.Elapsed += Title_LabelUpdater_Elapsed;
            Speed_LabelUpdater = new Timer
            {
                Interval = 200
            };
            Speed_LabelUpdater.Elapsed += Speed_LabelUpdater_Elapsed;


            workHorse = new BackgroundWorker();
            workHorse.DoWork += workHorse_DoWork;
            ProgressUpdater = new BackgroundWorker();
            ProgressUpdater.DoWork += ProgressUpdater_DoWork;

            #endregion
        }

        private void Encryptor_Loaded(object sender, RoutedEventArgs e)
        {
            Title_LabelUpdater.Start();
            lbl_source.Content = "Source:";
            lbl_source_path.Content = EncryptionData.SourceFileName;
            lbl_destination.Content = "Destination:";
            Title = @"Encrypting: " + EncryptionData.SourceFileName;
            ProgressUpdater.RunWorkerAsync();
            workHorse.RunWorkerAsync();
        }

        private void Encryptor_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Speed_LabelUpdater_Elapsed(object sender, ElapsedEventArgs e)
        {
            SpeedCalculator_Increment++;
            Dispatcher.Invoke(() =>
            {
                var sizeDiff = pb_progress.Value / SpeedCalculator_Increment;
                lbl_speed.Content = $"Speed: {Math.Round(sizeDiff * 5, 1)} MB/s";
            });
        }

        private void Title_LabelUpdater_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
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
        }

        private void workHorse_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var safeFileName = Path.GetFileNameWithoutExtension(EncryptionData.SourceFileName);
                var safeDirName = EncryptionData.SourceFileName.Remove(
                    EncryptionData.SourceFileName.Length - (safeFileName.Length +
                                                            Path.GetExtension(EncryptionData.SourceFileName).Length +
                                                            1),
                    safeFileName.Length + Path.GetExtension(EncryptionData.SourceFileName).Length + 1);
                var fileCount = new DirectoryInfo(safeDirName).GetFiles().Count(finf => finf.Name.StartsWith(Path.GetFileNameWithoutExtension(EncryptionData.SourceFileName) + "_encrypted"));

                if (fileCount == 0)
                    EncryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_encrypted" +
                                                         Path.GetExtension(EncryptionData.SourceFileName);
                else
                    EncryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_encrypted_" + fileCount +
                                                         Path.GetExtension(EncryptionData.SourceFileName);
                Dispatcher.Invoke(() =>
                {
                    lbl_destination_path.Content =
                        EncryptionData.DestinationFileName.Replace("_", "__"); //avoid mnemonics 
                });
                Speed_LabelUpdater.Start();
                Cryptography.Encryption.EncryptFile(EncryptionData.SourceFileName, EncryptionData.DestinationFileName,
                    1024);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ProgressUpdater_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var fileLength = Math.Round((double) new FileInfo(EncryptionData.SourceFileName).Length / 1048576, 0);
                var runloop = true;
                Dispatcher.Invoke(() => { pb_progress.Maximum = fileLength; });
                while (runloop)
                    if (File.Exists(EncryptionData.DestinationFileName))
                    {
                        Title_LabelUpdater.Start();
                        FileInfo finf = new(EncryptionData.DestinationFileName);
                        Dispatcher.Invoke(() =>
                        {
                            pb_progress.Value = Math.Round((double) finf.Length / 1048576, 0);
                            lbl_percentage.Content = Math.Round(pb_progress.Value / pb_progress.Maximum * 100, 0) + "%";
                            if (pb_progress.Value != pb_progress.Maximum) return;
                            Speed_LabelUpdater.Stop();
                            lbl_speed.Content = "Speed: --";
                            lbl_percentage.Content = "Finalizing file...";
                            lbl_title.Content = "Finishing up...";
                            runloop = false;
                        });
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}