using System;
using System.Windows;

namespace Backuper.UI.WPF.Services;
public class MessageBoxNotificationService : INotificationService {
    public event Action<string>? NewMessage;
    public void Send(string message) => MessageBox.Show(message);
    public void Send(string message, string title) => MessageBox.Show(message, title);
}
