namespace Backuper.Core.Services; 

public interface IBackuperServiceFactory {
    IBackuperService CreateBackuperService(string sourcePath);
}

