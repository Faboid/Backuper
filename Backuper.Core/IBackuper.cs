using Backuper.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.Core {
    public interface IBackuper {

        BackuperInfo Info { get; }

        //todo - use better returns to send back results to the UI
        Task EraseBackupsAsync();
        Task BinBackupsAsync();
        Task StartBackupAsync();

    }
}
