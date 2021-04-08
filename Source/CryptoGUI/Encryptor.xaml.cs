using CryptoAPI.ORM;
using CryptoGUI.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Encryptor.xaml
    /// </summary>
    public partial class Encryptor : Window
    {
        private readonly System.Timers.Timer Title_LabelUpdater;
        private readonly System.Timers.Timer Speed_LabelUpdater;
        private int SpeedCalculator_Increment = 0;

        private readonly BackgroundWorker workHorse;
        private readonly BackgroundWorker ProgressUpdater;
        public Encryptor()
        {
            InitializeComponent();
            Loaded += Encryptor_Loaded;
            Closing += Encryptor_Closing;
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
            this.Title = @"Encrypting: " + EncryptionData.SourceFileName;
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
            this.Dispatcher.Invoke(() =>
            {
                double sizeDiff = pb_progress.Value / SpeedCalculator_Increment;
                lbl_speed.Content = $"Speed: {Math.Round(sizeDiff * 5, 1)} MB/s";
            });

        }
        private void Title_LabelUpdater_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (lbl_title.Content.ToString() == "Encrypting")
                {
                    lbl_title.Content = "Encrypting.";
                }
                else if (lbl_title.Content.ToString() == "Encrypting.")
                {
                    lbl_title.Content = "Encrypting..";
                }
                else if (lbl_title.Content.ToString() == "Encrypting..")
                {
                    lbl_title.Content = "Encrypting...";
                }
                else if (lbl_title.Content.ToString() == "Encrypting...")
                {
                    lbl_title.Content = "Encrypting";
                }
            });

        }
        private void workHorse_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string safeFileName = System.IO.Path.GetFileNameWithoutExtension(EncryptionData.SourceFileName);
                string safeDirName = EncryptionData.SourceFileName.Remove(EncryptionData.SourceFileName.Length - (safeFileName.Length + System.IO.Path.GetExtension(EncryptionData.SourceFileName).Length + 1), (safeFileName.Length + System.IO.Path.GetExtension(EncryptionData.SourceFileName).Length + 1));
                int fileCount = 0;

                foreach (var finf in new DirectoryInfo(safeDirName).GetFiles())
                {
                    if (finf.Name.StartsWith(System.IO.Path.GetFileNameWithoutExtension(EncryptionData.SourceFileName) + "_encrypted"))
                    {
                        fileCount++;
                    }
                }

                if (fileCount == 0)
                {
                    EncryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_encrypted" + System.IO.Path.GetExtension(EncryptionData.SourceFileName);
                }
                else
                {
                    EncryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_encrypted_" + fileCount.ToString() + System.IO.Path.GetExtension(EncryptionData.SourceFileName);
                }
                this.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(EncryptionData.DestinationFileName);
                    lbl_destination_path.Content = EncryptionData.DestinationFileName.Replace("_", "__"); //avoid mnemonics 
                });
                File.WriteAllBytes(@"C:\users\albin\desktop\key.key", Cryptography.GenerateEncryptionKey(Cryptography.Encryption.HashPassword("ost123")));
                Speed_LabelUpdater.Start();
                Cryptography.Encryption.EncryptFile(EncryptionData.SourceFileName, EncryptionData.DestinationFileName, 2048 * 2048);
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

                double fileLength = Math.Round((double)new FileInfo(EncryptionData.SourceFileName).Length / 1048576, 0);
                bool runloop = true;
                this.Dispatcher.Invoke(() =>
                {

                    pb_progress.Maximum = fileLength;
                });
                while (runloop)
                {
                    if (File.Exists(EncryptionData.DestinationFileName))
                    {
                        Title_LabelUpdater.Start();
                        FileInfo finf = new(EncryptionData.DestinationFileName);
                        this.Dispatcher.Invoke(() =>
                        {
                            pb_progress.Value = Math.Round(((double)finf.Length / 1048576), 0);
                            this.lbl_percentage.Content = Math.Round((pb_progress.Value / pb_progress.Maximum) * 100, 0).ToString() + "%";
                            if (pb_progress.Value == pb_progress.Maximum)
                            {
                                runloop = false;
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}