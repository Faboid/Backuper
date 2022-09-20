using Backuper.Abstractions;
using Backuper.Utils;

namespace Backuper.Core.Services;

public class FileBackuperService : IBackuperService {

    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IFileInfo _source;

    public FileBackuperService(IFileInfo source, IDirectoryInfoProvider directoryInfoProvider) {
        _source = source;
        _directoryInfoProvider = directoryInfoProvider;
    }

    public async Task<BackupResult> BackupAsync(string newVersionPath, CancellationToken token = default) {
        _directoryInfoProvider.FromDirectoryPath(newVersionPath).Create();
        var filePath = Path.Combine(newVersionPath, _source.Name);
        await _source.CopyToAsync(filePath);
        return BackupResult.Success;
    }

    public Option<DateTime> GetSourceLastWriteTimeUTC() {
        return _source.LastWriteTimeUtc;
    }
}
