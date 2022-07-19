namespace Backuper.Core.Saves.DBConnections {
    public class MemoryDBConnection : IDBConnection {

        readonly Dictionary<string, string[]> dict = new();
        
        public void Delete(string path) {
            dict.Remove(path);
        }

        public bool Exists(string path) {
            return dict.ContainsKey(path);
        }

        public Task<string[]> ReadAllLinesAsync(string path) {
            return Task.FromResult(dict[path]);
        }

        public Task WriteAllLinesAsync(string path, string[] lines) {
            if(!dict.TryGetValue(path, out var newLines)) {
                newLines = lines;
                dict.Add(path, newLines);
            }

            dict[path] = newLines;
            return Task.CompletedTask;
        }
    }
}
