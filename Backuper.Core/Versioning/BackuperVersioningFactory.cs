using Backuper.Core.Services;
namespace Backuper.Core.Versioning;

public class BackuperVersioningFactory : IBackuperVersioningFactory {

    private readonly IPathsBuilderService pathsBuilderService;

    public BackuperVersioningFactory(IPathsBuilderService pathsBuilderService) {
        this.pathsBuilderService = pathsBuilderService;
    }

    public IBackuperVersioning CreateVersioning(string backuperName) {
        return new BackuperVersioning(backuperName, pathsBuilderService);
    }

}
