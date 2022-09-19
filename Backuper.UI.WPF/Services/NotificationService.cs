using System;

namespace Backuper.UI.WPF.Services;

public class NotificationService : INotificationService {

    public event Action<string>? NewMessage;
    public string Message { get; private set; } = "";

    private void OnNewMessage(string message) {
        NewMessage?.Invoke(message);
    }

    public void Send(string message) {
        OnNewMessage(new(message));
    }

    public void Send(string message, string title) {
        OnNewMessage($"{title}:{Environment.NewLine}{message}");
    }
}