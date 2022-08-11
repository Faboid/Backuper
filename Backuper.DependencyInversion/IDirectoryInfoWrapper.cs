using Backuper.Extensions;

namespace Backuper.DependencyInversion;

public interface IDirectoryInfoProvider {

    IDirectoryInfoWrapper Create(string path);

}

public interface IDirectoryInfoWrapper {

    IEnumerable<IDirectoryInfoWrapper> EnumerateDirectories(string searchPattern, SearchOption searchOption);
    IEnumerable<IDirectoryInfoWrapper> EnumerateDirectories();
    Task CopyToAsync(string path);
    void Delete(bool recursively);

}

public class DirectoryInfoFactory : IDirectoryInfoProvider {
    public IDirectoryInfoWrapper Create(string path) {
        return new DirectoryInfoWrapper(path);
    }
}

public class DirectoryInfoWrapper : IDirectoryInfoWrapper {

    private readonly DirectoryInfo _info;

    public DirectoryInfoWrapper(string path) {
        _info = new DirectoryInfo(path);
    }

    public DirectoryInfoWrapper(DirectoryInfo info) {
        _info = info;
    }

    public Task CopyToAsync(string path) {
        return _info.CopyToAsync(path);
    }

    public void Delete(bool recursively) {
        _info.Delete(recursively);
    }

    public IEnumerable<IDirectoryInfoWrapper> EnumerateDirectories(string searchPattern, SearchOption searchOption) {
        return _info.EnumerateDirectories(searchPattern, searchOption).Select(x => new DirectoryInfoWrapper(x));
    }

    public IEnumerable<IDirectoryInfoWrapper> EnumerateDirectories() {
        return _info.EnumerateDirectories().Select(x => new DirectoryInfoWrapper(x));
    }

}