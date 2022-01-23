using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using Microsoft.Win32;
using varbyte.encryption.Interfaces;
using varbyte.encryption.ORM;
using varbyte.encryption.Service;

namespace varbyte.gui.View.Dialogues;

public partial class NewKeyDlg : Window
{
    private readonly Services Services;
    private readonly string _baseDir;
    public bool Canceled = true;
    public NewKeyDlg(Services services)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        _baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#pragma warning restore CS8601 // Possible null reference assignment.
        Services = services;
        InitializeComponent();
    }


    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        AddKey();
    }
    private void AddKey()
    {
        if (pw_key.Password != "")
        {
            Canceled = false;
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Varbyte Encryption Key(*.varbyte)|*.varbyte";
            saveFileDialog.Title = "Varbyte - Select destination";
            saveFileDialog.AddExtension = true;
            if (saveFileDialog.ShowDialog() == true)
            {

                var success = GenerateKey(saveFileDialog.FileName);
                if (success)
                {
                    Services.ConfigurationManager.SetPrimaryKeyPath(saveFileDialog.FileName);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    DialogResult = false;
                }

            }
        }
    }
    private bool GenerateKey(string path)
    {
        var success = false;
        var iterations = 0;
        do
        {
            if (File.Exists(path)) File.Delete(path);
            System.Threading.Thread.Sleep(100);
            Services.CryptographyKeyService.WriteCryptographyKey(
                 Services.CryptographyKeyService.GenerateEncryptionKey(pw_key.Password), path);
            System.Threading.Thread.Sleep(100);
            success = ValidateKeyHealth(path, pw_key.Password);
            if (success)
                break;
            iterations++;

        } while (iterations <= 5);
        return success;
    }
    public bool ValidateKeyHealth(string keyPath, string password)
    {
        var rawFileName = _baseDir + @"\rawDataTestFile.temp";
        var encryptedFileName = _baseDir + @"\enc_dataTestFile.temp";
        var decryptedFileName = _baseDir + @"\dec_dataTestFile.temp";
        File.WriteAllTextAsync(rawFileName, "I love cheese").Wait();
        try
        {

            var key = Services.CryptographyKeyService.ReadKey(keyPath, password);
            if (key == null)
            {
                File.Delete(rawFileName);
                return false;
            }
            Services.CryptographyService.EncryptFile(rawFileName, encryptedFileName, key, 1);
            Services.CryptographyService.DecryptFile(encryptedFileName, decryptedFileName, key, 1);
            var result = File.ReadAllLines(decryptedFileName)[0];
            if (result == "I love cheese")
            {
                if (File.Exists(rawFileName)) File.Delete(rawFileName);
                if (File.Exists(encryptedFileName)) File.Delete(encryptedFileName);
                if (File.Exists(decryptedFileName)) File.Delete(decryptedFileName);
                return true;
            }

            else
            {
                if (File.Exists(rawFileName)) File.Delete(rawFileName);
                if (File.Exists(encryptedFileName)) File.Delete(encryptedFileName);
                if (File.Exists(decryptedFileName)) File.Delete(decryptedFileName);
                return false;
            }

        }
        catch (Exception)
        {

            if (File.Exists(rawFileName)) File.Delete(rawFileName);
            if (File.Exists(encryptedFileName)) File.Delete(encryptedFileName);
            if (File.Exists(decryptedFileName)) File.Delete(decryptedFileName);
            return false;
        }
    }

    private void pw_key_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if(e.Key == System.Windows.Input.Key.Enter)
        {
            e.Handled = false;
            AddKey();
        }
    }
}