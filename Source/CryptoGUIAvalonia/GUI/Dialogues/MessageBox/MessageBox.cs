using Avalonia.Controls;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CryptoGUIAvalonia.GUI.Dialogues.MessageBox
{
    public static class MessageBox
    {
        
        public static MessageBoxWindow.MessageBoxResult Show(Window owner, string message)
        {
            var messageBox = new MessageBoxWindow(message);
            messageBox.Show();
            return messageBox.DialogResult;
        }
        public static void Show(Window owner, string message, string windowTitle, string messageTitle)
        {
            
        }
        public static void Show(Window owner, string message, string windowTitle, string messageTitle, MessageBoxWindow.MessageBoxButtons buttons)
        {
            
        }
    }
}