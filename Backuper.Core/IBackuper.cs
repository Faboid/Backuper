using Backuper.Core.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Backuper.Core.Tests")]
namespace Backuper.Core;

public interface IBackuper : IDisposable {

    public string Name { get; }
    public string SourcePath { get; }
    public int MaxVersions { get; }
    public bool UpdateOnBoot { get; }

    Task<BackupResponseCode> BackupAsync(CancellationToken token = default);
    Task<EditBackuperResponseCode> EditAsync(BackuperInfo newInfo);
    Task BinAsync();
    public bool IsUpdated();

}
