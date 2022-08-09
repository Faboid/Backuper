using Backuper.Core.Services;
namespace Backuper.Core.Versioning;

public class BackuperVersioningFactory : IBackuperVersioningFactory {

    private readonly string mainDirectory;
    private readonly IPathsBuilderService pathsBuilderService;

    public BackuperVersioningFactory(string mainDirectory, IPathsBuilderService pathsBuilderService) {
        this.mainDirectory = mainDirectory;
        this.pathsBuilderService = pathsBuilderService;
    }

    public IBackuperVersioning CreateVersioning(string backuperName) {
        return new BackuperVersioning(mainDirectory, backuperName, pathsBuilderService);
    }

}
