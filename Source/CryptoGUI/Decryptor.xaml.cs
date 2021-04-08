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
    /// Interaction logic for Decryptor.xaml
    /// </summary>
    public partial class Decryptor : Window
    {
        private readonly System.Timers.Timer Title_LabelUpdater;
        private readonly System.Timers.Timer Speed_LabelUpdater;
        private int SpeedCalculator_Increment = 0;

        private readonly BackgroundWorker workHorse;
        private readonly BackgroundWorker ProgressUpdater;
        public Decryptor()
        {
            InitializeComponent();
            Loaded += Decryptor_Loaded;
            Closing += Decryptor_Closing;

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
        private void Decryptor_Loaded(object sender, RoutedEventArgs e)
        {
            Title_LabelUpdater.Start();
            lbl_source.Content = "Source:";
            lbl_source_path.Content = DecryptionData.SourceFileName;
            lbl_destination.Content = "Destination:";
            this.Title = @"Decrypting: " + DecryptionData.SourceFileName;
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
            });

        }
        private void workHorse_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string safeFileName = System.IO.Path.GetFileNameWithoutExtension(DecryptionData.SourceFileName);
                string safeDirName = DecryptionData.SourceFileName.Remove(DecryptionData.SourceFileName.Length - (safeFileName.Length + System.IO.Path.GetExtension(DecryptionData.SourceFileName).Length + 1), (safeFileName.Length + System.IO.Path.GetExtension(DecryptionData.SourceFileName).Length + 1));
                int fileCount = 0;

                foreach (var finf in new DirectoryInfo(safeDirName).GetFiles())
                {
                    if (finf.Name.StartsWith(System.IO.Path.GetFileNameWithoutExtension(DecryptionData.SourceFileName) + "_decrypted"))
                    {
                        fileCount++;
                    }
                }

                if (fileCount == 0)
                {
                    DecryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_decrypted" + System.IO.Path.GetExtension(DecryptionData.SourceFileName);
                }
                else
                {
                    DecryptionData.DestinationFileName = safeDirName + @"\" + safeFileName + "_decrypted_" + fileCount.ToString() + System.IO.Path.GetExtension(DecryptionData.SourceFileName);
                }
                this.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(DecryptionData.DestinationFileName);
                    lbl_destination_path.Content = DecryptionData.DestinationFileName.Replace("_", "__"); //avoid mnemonics 
                });
                File.WriteAllBytes(@"C:\users\albin\desktop\key.key", Cryptography.GenerateEncryptionKey(Cryptography.Encryption.HashPassword("ost123")));
                Speed_LabelUpdater.Start();
                Cryptography.Decryption.DecryptFile(DecryptionData.SourceFileName, DecryptionData.DestinationFileName, 1024);
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

                double fileLength = Math.Round((double)new FileInfo(DecryptionData.SourceFileName).Length / 1048576, 0);
                bool runloop = true;
                this.Dispatcher.Invoke(() =>
                {

                    pb_progress.Maximum = fileLength;
                });
                while (runloop)
                {
                    if (File.Exists(DecryptionData.DestinationFileName))
                    {
                        Title_LabelUpdater.Start();
                        FileInfo finf = new(DecryptionData.DestinationFileName);
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
