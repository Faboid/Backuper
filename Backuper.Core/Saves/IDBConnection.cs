namespace Backuper.Core.Saves;

internal interface IDBConnection {

    bool Exists(string name);
    IEnumerable<string> EnumerateNames();
    Task WriteAllLinesAsync(string name, string[] lines);
    Task<string[]> ReadAllLinesAsync(string name);
    void Delete(string name);

}

