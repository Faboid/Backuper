namespace Backuper.Core.Saves.DBConnections; 

public class FileDBConnection : IDBConnection {

    public FileDBConnection() : this(Path.Combine(Directory.GetCurrentDirectory(), "Backupers")) { }
    internal FileDBConnection(string customPath) {
        directoryPath = new(customPath);
        directoryPath.Create();
    }

    private readonly DirectoryInfo directoryPath;
    private string GetBackuperPath(string name) => Path.Combine(directoryPath.FullName, $"{name}.txt");

    public bool Exists(string name) {
        var path = GetBackuperPath(name);
        return File.Exists(path);
    }

    public Task<string[]> ReadAllLinesAsync(string name) {
        var path = GetBackuperPath(name);
        return File.ReadAllLinesAsync(path);
    }

    public Task WriteAllLinesAsync(string name, string[] lines) {
        var path = GetBackuperPath(name);
        return File.WriteAllLinesAsync(path, lines);
    }

    public void Delete(string name) {
        var path = GetBackuperPath(name);
        File.Delete(path);
    }

    public IEnumerable<string> EnumerateNames() {
        return directoryPath
            .EnumerateFiles()
            .Select(x => Path.GetFileNameWithoutExtension(x.FullName));
    }
}