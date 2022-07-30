using Backuper.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.Core {
    public interface IBackuper : IDisposable {

        BackuperInfo Info { get; }
        bool IsUpdated { get; }

        //todo - use better returns to send back results to the UI
        Task EraseBackupsAsync(CancellationToken token = default);
        Task BinBackupsAsync(CancellationToken token = default);
        Task StartBackupAsync(CancellationToken token = default);

    }
}
