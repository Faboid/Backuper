using Backuper.Core.Models;
using Backuper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.Core {
    public class BackuperConnection {

        public BackuperConnection() : this(Path.Combine(Directory.GetCurrentDirectory(), "Backupers")) { }

        internal BackuperConnection(string customPath) {
            directoryPath = new(customPath);
        }

        private readonly DirectoryInfo directoryPath;
        private string GetBackuperPath(string name) => Path.Combine(directoryPath.FullName, $"{name}.txt");

        public Task CreateBackuperAsync(BackuperInfo info) {
            var path = GetBackuperPath(info.Name);
            if(File.Exists(path)) {
                throw new ArgumentException(); //todo - use options instead of exceptions
            }
            var strings = info.ToStrings();
            return File.WriteAllLinesAsync(path, strings);
        }

        public async Task<BackuperInfo> GetBackuperAsync(string name) {
            var path = GetBackuperPath(name);
            var lines = await File.ReadAllLinesAsync(path);
            return BackuperInfo.Parse(lines);
        }

        public IAsyncEnumerable<BackuperInfo> GetAllBackupersAsync() {
            return directoryPath
                .EnumerateFiles()
                .SelectAsync(x => File.ReadAllLinesAsync(x.FullName))
                .Select(x => BackuperInfo.Parse(x));
        }

        public async Task UpdateBackuperAsync(string name, string? newName = null, int newMaxVersions = 0, bool? newUpdateOnBoot = null) {
            var path = GetBackuperPath(name);
            if(!File.Exists(path)) {
                throw new FileNotFoundException();
            }
            var curr = await GetBackuperAsync(name);
            curr.Name = string.IsNullOrWhiteSpace(newName) ? curr.Name : newName;
            curr.MaxVersions = newMaxVersions <= 0 ? curr.MaxVersions : newMaxVersions;
            curr.UpdateOnBoot = newUpdateOnBoot ?? curr.UpdateOnBoot;

            var newValues = curr.ToStrings();
            await File.WriteAllLinesAsync(path, newValues);
        }

        public void DeleteBackuper(string name) {
            var path = GetBackuperPath(name);
            File.Delete(path);
        }

    }
}
