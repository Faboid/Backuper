using Backuper.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.Core.Saves
{
    public interface IBackuperConnection
    {

        bool Exists(string name);
        Task SaveAsync(BackuperInfo info);
        Task<BackuperInfo> GetAsync(string name);
        IAsyncEnumerable<BackuperInfo> GetAllBackupersAsync();
        Task OverwriteAsync(string name, BackuperInfo info);
        void Delete(string name);

    }
}
