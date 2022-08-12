using Backuper.Core.Services;
using Backuper.DependencyInversion;

namespace Backuper.Core.Versioning;

public class BackuperVersioningFactory : IBackuperVersioningFactory {

    private readonly IPathsBuilderService _pathsBuilderService;
    private readonly IDirectoryInfoProvider _directoryInfoProvider;

    public BackuperVersioningFactory(IPathsBuilderService pathsBuilderService, IDirectoryInfoProvider directoryInfoProvider) {
        _pathsBuilderService = pathsBuilderService;
        _directoryInfoProvider = directoryInfoProvider;
    }

    public IBackuperVersioning CreateVersioning(string backuperName) {
        return new BackuperVersioning(backuperName, _pathsBuilderService, _directoryInfoProvider);
    }

}
