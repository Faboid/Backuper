namespace Backuper.Core.Saves;

public interface IDBConnection {

    bool Exists(string path);
    Task WriteAllLinesAsync(string path, string[] lines);
    Task<string[]> ReadAllLinesAsync(string path);
    void Delete(string path);

}

