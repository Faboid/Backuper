using Backuper.Abstractions;
using Backuper.Core.Services;
using Microsoft.Extensions.Logging;

namespace Backuper.Core.Versioning;

public class BackuperVersioningFactory : IBackuperVersioningFactory {

    private readonly ILogger<IBackuperVersioning> _logger;
    private readonly IPathsBuilderService _pathsBuilderService;
    private readonly IDirectoryInfoProvider _directoryInfoProvider;

    public BackuperVersioningFactory(IPathsBuilderService pathsBuilderService, IDirectoryInfoProvider directoryInfoProvider, ILogger<IBackuperVersioning> logger) {
        _pathsBuilderService = pathsBuilderService;
        _directoryInfoProvider = directoryInfoProvider;
        _logger = logger;
    }

    public IBackuperVersioning CreateVersioning(string backuperName) {
        return new BackuperVersioning(backuperName, _pathsBuilderService, _directoryInfoProvider, _logger);
    }

}
