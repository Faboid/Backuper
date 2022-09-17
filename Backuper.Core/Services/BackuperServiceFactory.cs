using Backuper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Backuper.Core.Services;

public class BackuperServiceFactory : IBackuperServiceFactory {

    private readonly ILogger<IBackuperServiceFactory>? _logger;
    private readonly IDirectoryInfoProvider _directoryInfoProvider;
    private readonly IFileInfoProvider _fileInfoProvider;

    public BackuperServiceFactory(IDirectoryInfoProvider directoryInfoProvider, IFileInfoProvider fileInfoProvider, ILogger<IBackuperServiceFactory>? logger = null) {
        _directoryInfoProvider = directoryInfoProvider;
        _fileInfoProvider = fileInfoProvider;
        _logger = logger;
    }

    public IBackuperService CreateBackuperService(string sourcePath) {

        if(string.IsNullOrWhiteSpace(sourcePath)) {
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

        _logger?.LogWarning("Tried to instance a backuper service with a non-existing path: {Path}", sourcePath);
        throw new InvalidDataException($"The source path doesn't exist or is invalid: {sourcePath}");

    }
}
