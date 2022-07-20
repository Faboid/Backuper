namespace Backuper.Core.Saves.DBConnections {
    public class MemoryDBConnection : IDBConnection {

        readonly Dictionary<string, string[]> dict = new();
        
        public void Delete(string path) {
            dict.Remove(path);
        }

        public IEnumerable<string> EnumerateNames() {
            return dict.Keys;
        }

        public bool Exists(string name) {
            return dict.ContainsKey(name);
        }

        public Task<string[]> ReadAllLinesAsync(string name) {
            return Task.FromResult(dict[name]);
        }

        public Task WriteAllLinesAsync(string name, string[] lines) {
            if(!dict.TryGetValue(name, out var newLines)) {
                newLines = lines;
                dict.Add(name, newLines);
            }

            dict[name] = newLines;
            return Task.CompletedTask;
        }
    }
}
