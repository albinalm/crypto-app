using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CryptoGUIAvalonia.GUI.Dialogues.MessageBox
{
    public static class MessageBox
    {
        
        public static void Show(string message)
        {
            var messageBox = new MessageBoxWindow(message);
            messageBox.Show();
        }
        public static void Show(Window owner, string message, string windowTitle, string messageTitle)
        {
            
        }
        public static void Show(Window owner, string message, string windowTitle, string messageTitle, MessageBoxWindow.MessageBoxButtons buttons)
        {
            
        }
    }
}