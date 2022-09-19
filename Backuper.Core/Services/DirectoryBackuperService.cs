using Backuper.Abstractions;
using Backuper.Extensions;
using Backuper.Utils;

namespace Backuper.Core.Services;

public class DirectoryBackuperService : IBackuperService {

    private readonly IDirectoryInfo _source;

    public DirectoryBackuperService(IDirectoryInfo source) {
        _source = source;
    }

    public async Task<BackupResult> BackupAsync(string newVersionPath, CancellationToken token = default) {
        await _source.CopyToAsync(newVersionPath).ConfigureAwait(false);
        return BackupResult.Success;
    }

    public Option<DateTime> GetSourceLastWriteTimeUTC() {

        //windows doesn't update the parents' folders' last write time
        //therefore, it's needed to go through all children directories
        var latestChange = _source
            .EnumerateDirectories("*", SearchOption.AllDirectories)
            .Select(GetLatestBetweenFiles)
            .Where(x => x != default)
            .DefaultIfEmpty()
            .Max();

        return (latestChange > _source.LastWriteTimeUtc) ? latestChange : _source.LastWriteTimeUtc;

    }

    private DateTime GetLatestBetweenFiles(IDirectoryInfo directory) {
        return directory.EnumerateFiles().Select(x => x.LastWriteTimeUtc).DefaultIfEmpty().Max();
    }

}
