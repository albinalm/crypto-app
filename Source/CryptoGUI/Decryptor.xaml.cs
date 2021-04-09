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
    ///     Interaction logic for Decryptor.xaml
    /// </summary>
    public partial class Decryptor : Window
    {
        private readonly BackgroundWorker ProgressUpdater;
        private readonly Timer Speed_LabelUpdater;
        private readonly Timer Title_LabelUpdater;

        private readonly BackgroundWorker workHorse;
        private int SpeedCalculator_Increment;

        public Decryptor()
        {
            InitializeComponent();
            Loaded += Decryptor_Loaded;
            Closing += Decryptor_Closing;

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

        private void Decryptor_Loaded(object sender, RoutedEventArgs e)
        {
            Title_LabelUpdater.Start();
            lbl_source.Content = "Source:";
            lbl_source_path.Content = DecryptionData.SourceFileName;
            lbl_destination.Content = "Destination:";
            Title = @"Decrypting: " + DecryptionData.SourceFileName;
            ProgressUpdater.RunWorkerAsync();
            workHorse.RunWorkerAsync();
        }

        private void Decryptor_Closing(object sender, CancelEventArgs e)
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
                    "Decrypting" => "Decrypting.",
                    "Decrypting." => "Decrypting..",
                    "Decrypting.." => "Decrypting...",
                    "Decrypting..." => "Decrypting",
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
                var safeFileName = Path.GetFileNameWithoutExtension(DecryptionData.SourceFileName);
                var safeDirName = DecryptionData.SourceFileName.Remove(
                    DecryptionData.SourceFileName.Length - (safeFileName.Length +
                                                            Path.GetExtension(DecryptionData.SourceFileName).Length +
                                                            1),
                    safeFileName.Length + Path.GetExtension(DecryptionData.SourceFileName).Length + 1);
                var fileCount = new DirectoryInfo(safeDirName).GetFiles().Count(finf => 
                    finf.Name.StartsWith(Path.GetFileNameWithoutExtension(DecryptionData.SourceFileName) + "_decrypted"));

                if (fileCount == 0)
                    DecryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_decrypted" +
                                                         Path.GetExtension(DecryptionData.SourceFileName);
                else
                    DecryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_decrypted_" + fileCount +
                                                         Path.GetExtension(DecryptionData.SourceFileName);


                Dispatcher.Invoke(() =>
                {
                    lbl_destination_path.Content =
                        DecryptionData.DestinationFileName.Replace("_", "__"); //avoid mnemonics 
                });

                Speed_LabelUpdater.Start();
                Cryptography.Decryption.DecryptFile(DecryptionData.SourceFileName, DecryptionData.DestinationFileName,
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
                var fileLength = Math.Round((double) new FileInfo(DecryptionData.SourceFileName).Length / 1048576, 0);
                var runloop = true;
                Dispatcher.Invoke(() => { pb_progress.Maximum = fileLength; });
                while (runloop)
                    if (File.Exists(DecryptionData.DestinationFileName))
                    {
                        Title_LabelUpdater.Start();
                        FileInfo finf = new(DecryptionData.DestinationFileName);
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