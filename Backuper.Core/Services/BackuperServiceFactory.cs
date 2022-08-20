using Backuper.Abstractions;

namespace Backuper.Core.Services; 

public class BackuperServiceFactory : IBackuperServiceFactory {

    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IFileInfoProvider _fileInfoProvider;

    public BackuperServiceFactory(IDirectoryInfoProvider directoryInfoProvider, IFileInfoProvider fileInfoProvider) {
        _directoryInfoProvider = directoryInfoProvider;
        _fileInfoProvider = fileInfoProvider;
    }

    public IBackuperService CreateBackuperService(string sourcePath) {

        if(sourcePath == null) {
            throw new ArgumentNullException(nameof(sourcePath));
        }

        var dirInfo = _directoryInfoProvider.FromDirectoryPath(sourcePath);
        if(dirInfo.Exists) {
            return new DirectoryBackuperService(dirInfo);
        }

        var fileInfo = _fileInfoProvider.FromFilePath(sourcePath);
        if(fileInfo.Exists) {
            return new FileBackuperService(fileInfo);
        }

        throw new InvalidDataException($"The source path doesn't exist or is invalid: {sourcePath}");

    }
}
