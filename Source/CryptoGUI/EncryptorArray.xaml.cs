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
    ///     Interaction logic for EncryptorArray.xaml
    /// </summary>
    public partial class EncryptorArray : Window
    {
        private readonly Timer Speed_LabelUpdater;
        private readonly Timer Title_LabelUpdater;

        private readonly BackgroundWorker workHorse;
        private string CurrentFileSource = "";
        private int FilesEncrypted;
        private BackgroundWorker ProgressUpdater;
        private int SpeedCalculator_Increment;

        public EncryptorArray()
        {
            InitializeComponent();

            Loaded += EncryptorArray_Loaded;
            Closing += EncryptorArray_Closing;

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

            #endregion
        }

        private void EncryptorArray_Loaded(object sender, RoutedEventArgs e)
        {
            Title_LabelUpdater.Start();


            lbl_destination.Content = "Destination:";
            Title = @"Encrypting...";
            pb_total.Maximum = EncryptionData.Sources.Count - 1;
            workHorse.RunWorkerAsync();
        }

        private void EncryptorArray_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Speed_LabelUpdater_Elapsed(object sender, ElapsedEventArgs e)
        {
            SpeedCalculator_Increment++;
            Dispatcher.Invoke(() =>
            {
                var sizeDiff =
                    pb_current.Value / SpeedCalculator_Increment;
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
                foreach (var source in EncryptionData.Sources)
                {
                    CurrentFileSource = source;

                    Dispatcher.Invoke(() =>
                    {
                        pb_total.Value = FilesEncrypted;
                        lbl_percentage.Content = Math.Round(pb_current.Value / pb_current.Maximum * 100, 0) + "%";
                    });
                    SpeedCalculator_Increment = 0;
                    var safeFileName = Path.GetFileNameWithoutExtension(source);
                    var safeDirName =
                        source.Remove(source.Length - (safeFileName.Length + Path.GetExtension(source).Length + 1),
                            safeFileName.Length + Path.GetExtension(source).Length + 1);
                    var fileCount = new DirectoryInfo(safeDirName).GetFiles().Count(finf => finf.Name.StartsWith(Path.GetFileNameWithoutExtension(source) + "_encrypted"));
                    if (fileCount == 0)
                        EncryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_encrypted" +
                                                             Path.GetExtension(source);
                    else
                        EncryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_encrypted_" +
                                                             fileCount + Path.GetExtension(source);

                    Dispatcher.Invoke(() =>
                    {
                        Title = @"Encrypting files to: " + safeDirName;
                        lbl_title.Content = "Encrypting..";
                        lbl_destination_path.Content = safeDirName;
                    });


                    Speed_LabelUpdater.Start();
                    Dispatcher.Invoke(() => { lbl_currentFile.Content = safeFileName + Path.GetExtension(source); });

                    Dispatcher.Invoke(() => { pb_current.Value = 0; });


                    Title_LabelUpdater.Start();
                    if (ProgressUpdater != null)
                    {
                        if (!ProgressUpdater.IsBusy)
                        {
                            ProgressUpdater = new BackgroundWorker();
                            ProgressUpdater.DoWork += ProgressUpdater_DoWork;
                            ProgressUpdater.ProgressChanged += ProgressUpdater_ProgressChanged;
                            ProgressUpdater.WorkerReportsProgress = true;
                            ProgressUpdater.RunWorkerAsync();
                        }
                    }
                    else
                    {
                        ProgressUpdater = new BackgroundWorker();
                        ProgressUpdater.DoWork += ProgressUpdater_DoWork;
                        ProgressUpdater.ProgressChanged += ProgressUpdater_ProgressChanged;
                        ProgressUpdater.WorkerReportsProgress = true;
                        ProgressUpdater.RunWorkerAsync();
                    }

                    Cryptography.Encryption.EncryptFile(source, EncryptionData.DestinationFileName, 1024);
                    FilesEncrypted++;
                }

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
                var fileLength = Math.Round((double) new FileInfo(CurrentFileSource).Length / 1048576, 0);
                var runloop = true;
                Dispatcher.Invoke(() => { pb_current.Maximum = fileLength; });
                while (runloop)
                    if (File.Exists(EncryptionData.DestinationFileName))
                    {
                        FileInfo finf = new(EncryptionData.DestinationFileName);
                        Dispatcher.Invoke(() =>
                        {
                            pb_current.Value = Math.Round((double) finf.Length / 1048576, 0);
                            if (pb_current.Value != pb_current.Maximum) return;
                            if (FilesEncrypted == EncryptionData.Sources.Count - 1)
                            {
                            }

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
                MessageBox.Show(ex.Message);
            }
        }

        private void ProgressUpdater_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pb_current.Value = e.ProgressPercentage;
        }
    }
}