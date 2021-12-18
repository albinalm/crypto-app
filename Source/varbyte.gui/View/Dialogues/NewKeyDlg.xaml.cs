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
    public NewKeyDlg(Services services)
    {
        Services = services;
        InitializeComponent();
    }


    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        
        if (pw_key.Password != "")
        {
            var sdlg = new SaveFileDialog();
            sdlg.Filter = "Varbyte Encryption Key(*.varbyte)|*.varbyte";
            
            sdlg.AddExtension = true;
            if (sdlg.ShowDialog() == true)
            {
                Services.CryptographyKeyService.WriteCryptographyKey(
                    Services.CryptographyKeyService.GenerateEncryptionKey(pw_key.Password), sdlg.FileName);
                
                DialogResult = true;
                Close();
            }
         
        }
    }   
}