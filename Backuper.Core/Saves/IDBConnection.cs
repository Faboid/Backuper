namespace Backuper.Core.Saves;

internal interface IDBConnection {

    bool Exists(string path);
    IEnumerable<string> EnumerateNames();
    Task WriteAllLinesAsync(string path, string[] lines);
    Task<string[]> ReadAllLinesAsync(string path);
    void Delete(string path);

}

