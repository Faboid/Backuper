using Backuper.Core.Models;
namespace Backuper.Core.Rewrite; 

public interface IBackuper {

    Task<BackupResponseCode> BackupAsync(CancellationToken token = default);
    Task<EditBackuperResponseCode> EditAsync(BackuperInfo newInfo);
    Task BinAsync();

}
