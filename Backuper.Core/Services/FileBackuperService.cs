using Backuper.Extensions;

namespace Backuper.Core.Services; 

public class FileBackuperService : IBackuperService {

    private readonly FileInfo _source;

    public FileBackuperService(FileInfo source) {
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
