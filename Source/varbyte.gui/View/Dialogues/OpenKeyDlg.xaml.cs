using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using Microsoft.Win32;
using varbyte.encryption.Interfaces;
using varbyte.encryption.Models.Exceptions;
using varbyte.encryption.ORM;
using varbyte.encryption.Service;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace varbyte.gui.View.Dialogues;

public partial class OpenKeyDlg : Window
{
    private readonly Services Services;
    private readonly bool IsLoadingKey;
    private readonly string _baseDir;
    private string fileName = "";
    public bool Canceled = true;
    public bool DialogCanceled = true;
    private bool IncorrectPassword = false;
    public OpenKeyDlg(Services services, bool isLoadingKey)
    {
        Services = services;
        IsLoadingKey = isLoadingKey;
#pragma warning disable CS8601 // Possible null reference assignment.
        _baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#pragma warning restore CS8601 // Possible null reference assignment.
        ShowKeyDialog();
        InitializeComponent();

      
    }


    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        OpenKey();
    }
    private void OpenKey()
    {
        if (pw_key.Password != "")
        {
            IncorrectPassword = false;
            Canceled = false;
            try
            {
                var result = ValidateKeyHealth(fileName, pw_key.Password);
                if (!IncorrectPassword)
                {
                    DialogResult = result;
                    if (IsLoadingKey)
                    {
                        if (result)
                            Services.ConfigurationManager.SetPrimaryKeyPath(fileName);
                    }
                    Close();
                }
                else
                {
                    Canceled = true;
                    lbl_incorrectPassword.Visibility = Visibility.Visible;

                }
            }
            catch (Exception)
            {
                DialogResult = false;
                Close();
            }


        }
    }
    private bool ValidateKeyHealth(string keyPath, string password)
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
        catch (PasswordIncorrectException)
        {
            IncorrectPassword = true;
            if (File.Exists(rawFileName)) File.Delete(rawFileName);
            if (File.Exists(encryptedFileName)) File.Delete(encryptedFileName);
            if (File.Exists(decryptedFileName)) File.Delete(decryptedFileName);
            return false;
        }
        catch (Exception)
        {
            
            if (File.Exists(rawFileName)) File.Delete(rawFileName);
            if (File.Exists(encryptedFileName)) File.Delete(encryptedFileName);
            if (File.Exists(decryptedFileName)) File.Delete(decryptedFileName);
            return false;
        }
    
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      
    }
    private void ShowKeyDialog()
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Varbyte Encryption Key(*.varbyte)|*.varbyte";
        openFileDialog.Title = "Varbyte - Select key";
        openFileDialog.AddExtension = true;
        if (openFileDialog.ShowDialog() == true)
        {
            DialogCanceled = false;
            fileName = openFileDialog.FileName;
            this.Title = "Varbyte - Enter password";
           
        }
        else
        {
            Close();
        }
    }

    private void pw_key_GotFocus(object sender, RoutedEventArgs e)
    {
        lbl_incorrectPassword.Visibility = Visibility.Hidden;
    }

   

    private void pw_key_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if(e.Key == System.Windows.Input.Key.Enter)
        {
            e.Handled = false;
            OpenKey();
        }
    }
}