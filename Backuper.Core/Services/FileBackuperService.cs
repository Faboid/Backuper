using Backuper.Abstractions;

namespace Backuper.Core.Services;

public class FileBackuperService : IBackuperService {

    private readonly IFileInfo _source;

    public FileBackuperService(IFileInfo source) {
        _source = source;
    }

    public async Task BackupAsync(string newVersionPath, CancellationToken token = default) {
        var filePath = Path.Combine(newVersionPath, _source.Name);
        await _source.CopyToAsync(filePath);
    }

    public DateTime GetSourceLastWriteTimeUTC() {
        return _source.LastWriteTimeUtc;
    }
}
