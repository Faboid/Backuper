using Backuper.Extensions;

namespace Backuper.DependencyInversion;

public class DirectoryInfoWrapper : IDirectoryInfo {

    private readonly DirectoryInfo _info;

    public DirectoryInfoWrapper(string path) {
        _info = new DirectoryInfo(path);
    }

    public DirectoryInfoWrapper(DirectoryInfo info) {
        _info = info;
    }

    public string FullName => _info.FullName;
    public string Name => _info.Name;
    public DateTime CreationTimeUtc => _info.CreationTimeUtc;
    public bool Exists => _info.Exists;

    public Task CopyToAsync(string path) => _info.CopyToAsync(path);

    public void Delete(bool recursively) => _info.Delete(recursively);

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption) {
        return _info.EnumerateDirectories(searchPattern, searchOption).Select(x => new DirectoryInfoWrapper(x));
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories() {
        return _info.EnumerateDirectories().Select(x => new DirectoryInfoWrapper(x));
    }

    public IEnumerable<IFileInfo> EnumerateFiles() {
        return _info.EnumerateFiles().Select(x => new FileInfoWrapper(x));
    }

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption) {
        return _info.EnumerateFiles(searchPattern, searchOption).Select(x => new FileInfoWrapper(x));
    }

    public static implicit operator DirectoryInfoWrapper(DirectoryInfo info) => new(info);
    public static implicit operator DirectoryInfo(DirectoryInfoWrapper info) => info._info;

}