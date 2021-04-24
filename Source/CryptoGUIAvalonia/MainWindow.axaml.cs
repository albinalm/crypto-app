using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CryptoGUIAvalonia.GUI.Dialogues.MessageBox;

namespace CryptoGUIAvalonia
{
    public class MainWindow : Window
    {
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            foreach (var arg in ApplicationDataHolder.args)
            {
                MessageBox.Show(arg);
            }
        }

       
        private void ShowMessage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("lol");
        }
    }
}