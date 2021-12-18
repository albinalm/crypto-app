using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using Microsoft.Win32;
using varbyte.encryption.Interfaces;
using varbyte.encryption.ORM;
using varbyte.encryption.Service;

namespace varbyte.gui.View.Dialogues;

public partial class OpenKeyDlg : Window
{
    private readonly Services Services;

    public OpenKeyDlg(Services services)
    {
        Services = services;
        InitializeComponent();
    }


    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (pw_key.Password != "")
        {
            var safeFileDialog = new SaveFileDialog();
            safeFileDialog.Filter = "Varbyte Encryption Key(*.varbyte)|*.varbyte";

            safeFileDialog.AddExtension = true;
            if (safeFileDialog.ShowDialog() == true)
            {
                Services.CryptographyKeyService.WriteCryptographyKey(
                    Services.CryptographyKeyService.GenerateEncryptionKey(pw_key.Password), safeFileDialog.FileName);


                Services.CryptographyKeyService.SetPrimaryKeyPath(safeFileDialog.FileName);
                DialogResult = true;
                Close();
            }
        }
    }
}