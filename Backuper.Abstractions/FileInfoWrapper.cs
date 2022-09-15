using Backuper.Extensions;

namespace Backuper.Abstractions;

public class FileInfoWrapper : IFileInfo {

    private readonly FileInfo _info;

    public FileInfoWrapper(string path) {
        _info = new FileInfo(path);
    }

    public FileInfoWrapper(FileInfo info) {
        _info = info;
    }

    public string FullName => _info.FullName;
    public string Name => _info.Name;
    public DateTime CreationTimeUtc => _info.CreationTimeUtc;
    public DateTime LastWriteTimeUtc => _info.LastWriteTimeUtc;
    public bool Exists => _info.Exists;

    public void WriteAllLines(IEnumerable<string> lines) => File.WriteAllLines(_info.FullName, lines);
    public IEnumerable<string> ReadLines() => File.ReadLines(_info.FullName);

    public Task CopyToAsync(string path) {
        return _info.CopyToAsync(path);
    }
}