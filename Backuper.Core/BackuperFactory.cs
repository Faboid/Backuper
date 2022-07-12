using Backuper.Core.BackuperTypes;
using Backuper.Core.Models;
using Backuper.Utils;

namespace Backuper.Core; 

public class BackuperFactory {

    public BackuperFactory() {
        pathsBuilder = new();
    }

    public BackuperFactory(PathsBuilder pathsBuilder) {
        this.pathsBuilder = pathsBuilder;
    }

    private readonly PathsBuilder pathsBuilder;

    public IBackuper CreateBackuper(BackuperInfo info) {

        if(info == null) {
            throw new ArgumentNullException(nameof(info));
        }

        if(Directory.Exists(info.SourcePath)) {
            return new DirectoryBackuper(info, pathsBuilder);
        } 
        
        if(File.Exists(info.SourcePath)) {
            throw new NotImplementedException("The file backuper has yet to be implemented.");
        } 
        
        throw new InvalidDataException($"The source path doesn't exist or is invalid: {info.SourcePath}");
    }

}
