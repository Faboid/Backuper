using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackuperLibrary.UISpeaker {

    public static class MessageUI {

        public static event EventHandler<MailArgs> Mail;

#nullable enable
        public static void Send(object? sender, string title, string message) {
            var args = new MailArgs(title, message);
            Mail?.Invoke(sender, args);
        }
#nullable disable

    }

    public class MailArgs : EventArgs {

        public MailArgs(string title, string message) {
            Title = title;
            Message = message;
        }

        public string Title { get; set; }
        public string Message { get; set; }
    }

}
