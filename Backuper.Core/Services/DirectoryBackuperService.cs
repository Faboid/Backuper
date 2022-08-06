using Backuper.Extensions;

namespace Backuper.Core.Services; 

public class DirectoryBackuperService : IBackuperService {

    private readonly DirectoryInfo _source;

    public DirectoryBackuperService(DirectoryInfo source) {
        _source = source;
    }

    public async Task BackupAsync(string newVersionPath, CancellationToken token = default) {
        await _source.CopyToAsync(newVersionPath).ConfigureAwait(false);
    }

    public DateTime GetSourceLastWriteTimeUTC() {

        //windows doesn't update the parents' folders' last write time
        //therefore, it's needed to go through all children directories
        var latestChange = _source
            .EnumerateDirectories("*", SearchOption.AllDirectories)
            .Select(x => x.EnumerateFiles().Max(x => x.LastWriteTimeUtc))
            .DefaultIfEmpty()
            .Max();

        return latestChange;

    }
}
