using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.Services;
internal interface INotificationService {

    void Send(string message);
    void Send(string message, string title);

}
