using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Backuper.UI.WPF.Services;
public class MessageBoxNotificationService : INotificationService {
    public void Send(string message) => MessageBox.Show(message);
    public void Send(string message, string title) => MessageBox.Show(message, title);
}
