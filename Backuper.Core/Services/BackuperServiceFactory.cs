namespace Backuper.Core.Services; 

public class BackuperServiceFactory : IBackuperServiceFactory {
    public IBackuperService CreateBackuperService(string sourcePath) {

        if(sourcePath == null) {
            throw new ArgumentNullException(nameof(sourcePath));
        }

        if(Directory.Exists(sourcePath)) {
            return new DirectoryBackuperService(new(sourcePath));
        }

        if(File.Exists(sourcePath)) {
            return new FileBackuperService(new(sourcePath));
        }

        throw new InvalidDataException($"The source path doesn't exist or is invalid: {sourcePath}");

    }
}
