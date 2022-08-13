using Backuper.Extensions;

namespace Backuper.DependencyInversion;

public class DirectoryInfoWrapper : IDirectoryInfoWrapper {

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

    public IEnumerable<IDirectoryInfoWrapper> EnumerateDirectories(string searchPattern, SearchOption searchOption) {
        return _info.EnumerateDirectories(searchPattern, searchOption).Select(x => new DirectoryInfoWrapper(x));
    }

    public IEnumerable<IDirectoryInfoWrapper> EnumerateDirectories() {
        return _info.EnumerateDirectories().Select(x => new DirectoryInfoWrapper(x));
    }

    public static implicit operator DirectoryInfoWrapper(DirectoryInfo info) => new(info);
    public static implicit operator DirectoryInfo(DirectoryInfoWrapper info) => info._info;

}