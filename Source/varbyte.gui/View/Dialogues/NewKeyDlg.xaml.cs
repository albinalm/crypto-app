using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using Microsoft.Win32;
using varbyte.encryption.Interfaces;
using varbyte.encryption.ORM;

namespace varbyte.gui.View.Dialogues;

public partial class NewKeyDlg : Window
{

    private readonly ICryptography _cryptography;
    public NewKeyDlg(ICryptography cryptography)
    {
       
        _cryptography = cryptography;
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
                if(File.Exists(sdlg.FileName))
                    File.Delete(sdlg.FileName);
                var keyBytes = _cryptography.GenerateEncryptionKey(_cryptography.HashPassword(pw_key.Password));
                var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @$"\{Path.GetFileNameWithoutExtension(sdlg.FileName)}";
                Directory.CreateDirectory(path);
                File.WriteAllBytes(path + @"\value.key", keyBytes);
                File.WriteAllText(path + @"\value.val", _cryptography.HashPassword(pw_key.Password));
                ZipFile.CreateFromDirectory(path, sdlg.FileName, CompressionLevel.NoCompression, false);
                Directory.Delete(path, true);
                DialogResult = true;
                Close();
            }
         
        }
    }   
}