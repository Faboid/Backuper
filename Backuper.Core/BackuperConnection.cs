using Backuper.Core.Models;
using Backuper.Extensions;
using Backuper.Utils;

namespace Backuper.Core; 

public class BackuperConnection {

    public BackuperConnection() : this(Path.Combine(Directory.GetCurrentDirectory(), "Backupers")) { }

    internal BackuperConnection(string customPath) {
        directoryPath = new(customPath);
    }

    private readonly DirectoryInfo directoryPath;
    private string GetBackuperPath(string name) => Path.Combine(directoryPath.FullName, $"{name}.txt");

    //todo - consider whether to use a wrapper to validate the given parameters
    //or to check them here

    public async Task<CreateBackuperCode> CreateBackuperAsync(BackuperInfo info) {
        var path = GetBackuperPath(info.Name);
        if(File.Exists(path)) {
            return CreateBackuperCode.BackuperExistsAlready;
        }
        var strings = info.ToStrings();
        await File.WriteAllLinesAsync(path, strings);
        return CreateBackuperCode.Success;
    }

    public async Task<Option<BackuperInfo, GetBackuperCode>> GetBackuperAsync(string name) {
        var path = GetBackuperPath(name);
        if(!File.Exists(path)) {
            return GetBackuperCode.BackuperDoesNotExist;
        }
        var lines = await File.ReadAllLinesAsync(path);
        return BackuperInfo.Parse(lines);
    }

    public IAsyncEnumerable<BackuperInfo> GetAllBackupersAsync() {
        return directoryPath
            .EnumerateFiles()
            .SelectAsync(x => File.ReadAllLinesAsync(x.FullName))
            .Select(x => BackuperInfo.Parse(x)); //todo - error handling in case the data of that backuper was corrupted
    }

    public async Task<UpdateBackuperCode> UpdateBackuperAsync(string name, string? newName = null, int newMaxVersions = 0, bool? newUpdateOnBoot = null) {
        var path = GetBackuperPath(name);
        
        if(!File.Exists(path)) {
            return UpdateBackuperCode.BackuperDoesNotExist;
        }

        var curr = BackuperInfo.Parse(await File.ReadAllLinesAsync(path));
        curr.Name = string.IsNullOrWhiteSpace(newName) ? curr.Name : newName;
        curr.MaxVersions = newMaxVersions <= 0 ? curr.MaxVersions : newMaxVersions;
        curr.UpdateOnBoot = newUpdateOnBoot ?? curr.UpdateOnBoot;

        var newValues = curr.ToStrings();
        await File.WriteAllLinesAsync(path, newValues);
        return UpdateBackuperCode.Success;
    }

    public DeleteBackuperCode DeleteBackuper(string name) {
        var path = GetBackuperPath(name);
        if(!File.Exists(path)) {
            return DeleteBackuperCode.BackuperDoesNotExist;
        }

        File.Delete(path);
        return DeleteBackuperCode.Success;
    }

    public enum CreateBackuperCode {
        Unknown,
        Success,
        Failure,
        BackuperExistsAlready
    }

    public enum GetBackuperCode {
        Unknown,
        BackuperDoesNotExist
    }

    public enum UpdateBackuperCode {
        Unknown,
        Success,
        BackuperDoesNotExist
    }

    public enum DeleteBackuperCode {
        Unknown,
        Success,
        BackuperDoesNotExist
    }

}

