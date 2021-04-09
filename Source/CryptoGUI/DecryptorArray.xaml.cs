using CryptoAPI.ORM;
using CryptoGUI.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CryptoGUI
{
   
    /// <summary>
    /// Interaction logic for DecryptorArray.xaml
    /// </summary>
    public partial class DecryptorArray : Window
    {
        private readonly System.Timers.Timer Title_LabelUpdater;
        private readonly System.Timers.Timer Speed_LabelUpdater;
        private int SpeedCalculator_Increment = 0;

        private readonly BackgroundWorker workHorse;
        private BackgroundWorker ProgressUpdater;
        private string CurrentFileSource = "";
        private int FilesDecrypted = 0;
        public DecryptorArray()
        {
            InitializeComponent();
            Loaded += DecryptorArray_Loaded;
            Closing += DecryptorArray_Closing;
            #region Declaration and Event assigns

            Title_LabelUpdater = new()
            {
                Interval = 500
            };
            Title_LabelUpdater.Elapsed += Title_LabelUpdater_Elapsed;
            Speed_LabelUpdater = new()
            {
                Interval = 200
            };
            Speed_LabelUpdater.Elapsed += Speed_LabelUpdater_Elapsed;


            workHorse = new BackgroundWorker();
            workHorse.DoWork += workHorse_DoWork;
            #endregion
        }
        private void DecryptorArray_Loaded(object sender, RoutedEventArgs e)
        {
            Title_LabelUpdater.Start();


            lbl_destination.Content = "Destination:";
            this.Title = @"Decrypting...";
            pb_total.Maximum = DecryptionData.Sources.Count - 1;
           workHorse.RunWorkerAsync();
            

        }
        private void DecryptorArray_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }
        private void Speed_LabelUpdater_Elapsed(object sender, ElapsedEventArgs e)
        {
            SpeedCalculator_Increment++;
            this.Dispatcher.Invoke(() =>
            {
                double sizeDiff =
                pb_current.Value / SpeedCalculator_Increment;
                lbl_speed.Content = $"Speed: {Math.Round(sizeDiff * 5, 1)} MB/s";
            });
        }
        private void Title_LabelUpdater_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (lbl_title.Content.ToString() == "Decrypting")
                {
                    lbl_title.Content = "Decrypting.";
                }
                else if (lbl_title.Content.ToString() == "Decrypting.")
                {
                    lbl_title.Content = "Decrypting..";
                }
                else if (lbl_title.Content.ToString() == "Decrypting..")
                {
                    lbl_title.Content = "Decrypting...";
                }
                else if (lbl_title.Content.ToString() == "Decrypting...")
                {
                    lbl_title.Content = "Decrypting";
                }
                else if (lbl_title.Content.ToString() == "Finishing up")
                {
                    lbl_title.Content = "Finishing up.";
                }
                else if (lbl_title.Content.ToString() == "Finishing up.")
                {
                    lbl_title.Content = "Finishing up..";
                }
                else if (lbl_title.Content.ToString() == "Finishing up..")
                {
                    lbl_title.Content = "Finishing up...";
                }
                else if (lbl_title.Content.ToString() == "Finishing up...")
                {
                    lbl_title.Content = "Finishing up";
                }
            });

        }
        private void workHorse_DoWork(object sender, DoWorkEventArgs e)
        {


            try
            {
                foreach (var source in DecryptionData.Sources)
                {

                    CurrentFileSource = source;

                    this.Dispatcher.Invoke(() =>
                    {
                        pb_total.Value = FilesDecrypted;
                        lbl_percentage.Content = Math.Round((pb_current.Value / pb_current.Maximum) * 100, 0).ToString() + "%";
                    });
                    SpeedCalculator_Increment = 0;
                    string safeFileName = System.IO.Path.GetFileNameWithoutExtension(source);
                    string safeDirName = source.Remove(source.Length - (safeFileName.Length + System.IO.Path.GetExtension(source).Length + 1), (safeFileName.Length + System.IO.Path.GetExtension(source).Length + 1));
                    int fileCount = 0;
                    foreach (var finf in new DirectoryInfo(safeDirName).GetFiles())
                    {
                        if (finf.Name.StartsWith(System.IO.Path.GetFileNameWithoutExtension(source) + "_decrypted"))
                        {
                            fileCount++;
                        }
                    }
                    if (fileCount == 0)
                    {
                        DecryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_decrypted" + System.IO.Path.GetExtension(source);
                    }
                    else
                    {
                        DecryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_decrypted_" + fileCount.ToString() + System.IO.Path.GetExtension(source);
                    }

                    this.Dispatcher.Invoke(() =>
                    {
                        this.Title = @"Decrypting files to: " + safeDirName;
                        lbl_title.Content = "Decrypting..";
                        lbl_destination_path.Content = safeDirName;

                    });
                    Cryptography.ReadEncryptionKey(Cryptography.Encryption.HashPassword("ost123"), File.ReadAllBytes(@"C:\users\albin\desktop\key.key"));

                    Speed_LabelUpdater.Start();
                    this.Dispatcher.Invoke(() =>
                    {
                        lbl_currentFile.Content = safeFileName + System.IO.Path.GetExtension(source);
                    });

                    this.Dispatcher.Invoke(() =>
                    {

                        pb_current.Value = 0;
                    });


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
                    Cryptography.Decryption.DecryptFile(source, DecryptionData.DestinationFileName, 1024);
                    FilesDecrypted++;
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

                double fileLength = Math.Round((double)new FileInfo(CurrentFileSource).Length / 1048576, 0);
                bool runloop = true;
                this.Dispatcher.Invoke(() =>
                {

                    pb_current.Maximum = fileLength;
                });
                while (runloop)
                {
                    if (File.Exists(DecryptionData.DestinationFileName))
                    {
                        FileInfo finf = new(DecryptionData.DestinationFileName);
                        this.Dispatcher.Invoke(() =>
                        {
                            pb_current.Value = Math.Round(((double)finf.Length / 1048576), 0);
                            if (pb_current.Value == pb_current.Maximum)
                            {
                                if (FilesDecrypted == DecryptionData.Sources.Count - 1)
                                {

                                }
                                Speed_LabelUpdater.Stop();
                                lbl_speed.Content = "Speed: --";
                                lbl_percentage.Content = "Finalizing file...";
                                lbl_title.Content = "Finishing up...";
                                runloop = false;
                            }
                        });
                    }
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
