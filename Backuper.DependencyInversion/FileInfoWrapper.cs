using Backuper.Extensions;

namespace Backuper.DependencyInversion; 

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
    public bool Exists => _info.Exists;

    public Task CopyToAsync(string path) {
        return _info.CopyToAsync(path);
    }
}