using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CryptoGUIAvalonia.GUI.Dialogues.MessageBox
{
    public class MessageBoxWindow : Window
    {
        public MessageBoxResult DialogResult { get; set; }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
        }
        public MessageBoxWindow(string windowTitle, string messageTitle, string message, MessageBoxButtons buttons)
        {
            InitializeComponent();
            #if DEBUG
            this.AttachDevTools();
            #endif
            var titleLabel = this.Get<Label>("TitleLabel");
            var messageLabel = this.Get<Label>("MessageLabel");
            titleLabel.Content = messageTitle;
            messageLabel.Content = message;
            Title = windowTitle;
            switch (buttons)
            {
                case MessageBoxButtons.Ok:
                    this.Get<Button>("BtnOk").IsVisible = true;
                    break;
                case MessageBoxButtons.YesNo:
                    this.Get<Button>("BtnYes").IsVisible = true;
                    this.Get<Button>("BtnNo").IsVisible = true;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    this.Get<Button>("BtnYes").IsVisible = true;
                    this.Get<Button>("BtnNo").IsVisible = true;
                    this.Get<Button>("BtnCancel").IsVisible = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
            }
        }
        public MessageBoxWindow(string windowTitle, string messageTitle, string message)
        {
            InitializeComponent();
            #if DEBUG
            this.AttachDevTools();
            #endif
            var titleLabel = this.Get<Label>("TitleLabel");
            var messageLabel = this.Get<Label>("MessageLabel");
            titleLabel.Content = messageTitle;
            messageLabel.Content = message;
            Title = windowTitle;
            this.Get<Button>("BtnOk").IsVisible = true;
        }
        public MessageBoxWindow(string message)
        {
            InitializeComponent();
            
            #if DEBUG
            this.AttachDevTools();
            #endif
            this.Get<Label>("TitleLabel").IsVisible = false;
            this.Get<Label>("MessageLabel").Content = message;
            Title = message;
            this.Get<Button>("BtnOk").IsVisible = true;
        }
        public MessageBoxWindow()
        {
            InitializeComponent();
            #if DEBUG
            this.AttachDevTools();
            #endif
        }

        public enum MessageBoxButtons
        {
            Ok,
            YesNo,
            YesNoCancel
        }

        public enum MessageBoxResult
        {
            Ok,
            Yes,
            No,
            Cancel
        }
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = MessageBoxResult.Ok;
            Close();
        }
        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = MessageBoxResult.Yes;
            Close();
        }
        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = MessageBoxResult.No;
            Close();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = MessageBoxResult.Cancel;
            Close();
        }
    }
}