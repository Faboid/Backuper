using Backuper.Core.Models;
namespace Backuper.Core.Rewrite; 

public interface IBackuper {

    public string Name { get; }
    public string SourcePath { get; }
    public int MaxVersions { get; }
    public bool UpdateOnBoot { get; }

    Task<BackupResponseCode> BackupAsync(CancellationToken token = default);
    Task<EditBackuperResponseCode> EditAsync(BackuperInfo newInfo);
    Task BinAsync();

}
