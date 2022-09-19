using Backuper.Core.Models;
using Backuper.Utils;

namespace Backuper.Core.Saves; 

public interface IBackuperConnection {

    bool Exists(string name);
    Task SaveAsync(BackuperInfo info);
    Task<BackuperInfo> GetAsync(string name);
    IAsyncEnumerable<Option<BackuperInfo, string>> GetAllBackupersAsync();
    Task OverwriteAsync(string name, BackuperInfo info);
    void Delete(string name);

}
