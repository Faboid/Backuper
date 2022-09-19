using Backuper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Backuper.Core.Services;

public class BackuperServiceFactory : IBackuperServiceFactory {

    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IFileInfoProvider _fileInfoProvider;

    public BackuperServiceFactory(IDirectoryInfoProvider directoryInfoProvider, IFileInfoProvider fileInfoProvider) {
        _directoryInfoProvider = directoryInfoProvider;
        _fileInfoProvider = fileInfoProvider;
    }

    public IBackuperService CreateBackuperService(string sourcePath) {

        if(string.IsNullOrWhiteSpace(sourcePath)) {
            throw new ArgumentNullException(nameof(sourcePath));
        }

        if(!IsPathValid(sourcePath)) {
            throw new InvalidDataException("Must give a valid path.");
        }

        var dirInfo = _directoryInfoProvider.FromDirectoryPath(sourcePath);
        if(dirInfo.Exists) {
            return new DirectoryBackuperService(dirInfo);
        }

        var fileInfo = _fileInfoProvider.FromFilePath(sourcePath);
        if(fileInfo.Exists) {
            return new FileBackuperService(fileInfo, _directoryInfoProvider);
        }

        return new HibernatingBackuperService();

    }

    public IBackuperService CreateCorruptedService() => new CorruptedBackuperService();

    private static bool IsPathValid(string newPath) {

        return
            !string.IsNullOrWhiteSpace(newPath)
            && Path.IsPathRooted(newPath)
            && newPath.Where(x => x == ':').Count() == 1
            && !Path.GetInvalidPathChars().Any(x => newPath.Contains(x));
    }

}
