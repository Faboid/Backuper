namespace Backuper.Core.Saves.DBConnections; 

public class FileDBConnection : IDBConnection {
    public bool Exists(string path) {
        return File.Exists(path);
    }

    public Task<string[]> ReadAllLinesAsync(string path) {
        return File.ReadAllLinesAsync(path);
    }

    public Task WriteAllLinesAsync(string path, string[] lines) {
        return File.WriteAllLinesAsync(path, lines);
    }

    public void Delete(string path) {
        File.Delete(path);
    }
}