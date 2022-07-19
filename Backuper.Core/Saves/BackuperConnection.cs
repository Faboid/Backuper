using Backuper.Core.Models;
using Backuper.Core.Saves.DBConnections;
using Backuper.Extensions;
using Backuper.Utils;

namespace Backuper.Core.Saves;

public class BackuperConnection {

    public BackuperConnection() : this(new FileDBConnection()) { }
    public BackuperConnection(IDBConnection dbConnection) : this(dbConnection, Path.Combine(Directory.GetCurrentDirectory(), "Backupers")) { }
    internal BackuperConnection(IDBConnection dbConnection, string customPath) {
        directoryPath = new(customPath);
        this.dbConnection = dbConnection;
    }

    private readonly IDBConnection dbConnection;
    private readonly DirectoryInfo directoryPath;
    private string GetBackuperPath(string name) => Path.Combine(directoryPath.FullName, $"{name}.txt");

    //todo - consider whether to use a wrapper to validate the given parameters
    //or to check them here

    public async Task<CreateBackuperCode> CreateBackuperAsync(BackuperInfo info) {
        var path = GetBackuperPath(info.Name);
        if(dbConnection.Exists(path)) {
            return CreateBackuperCode.BackuperExistsAlready;
        }
        var strings = info.ToStrings();
        await dbConnection.WriteAllLinesAsync(path, strings);
        return CreateBackuperCode.Success;
    }

    public async Task<Option<BackuperInfo, GetBackuperCode>> GetBackuperAsync(string name) {
        var path = GetBackuperPath(name);
        if(!dbConnection.Exists(path)) {
            return GetBackuperCode.BackuperDoesNotExist;
        }
        var lines = await dbConnection.ReadAllLinesAsync(path);
        return BackuperInfo.Parse(lines);
    }

    public IAsyncEnumerable<BackuperInfo> GetAllBackupersAsync() {
        return directoryPath
            .EnumerateFiles()
            .SelectAsync(x => dbConnection.ReadAllLinesAsync(x.FullName))
            .Select(x => BackuperInfo.Parse(x)); //todo - error handling in case the data of that backuper was corrupted
    }

    public async Task<UpdateBackuperCode> UpdateBackuperAsync(string name, string? newName = null, int newMaxVersions = 0, bool? newUpdateOnBoot = null) {
        var path = GetBackuperPath(name);

        if(!dbConnection.Exists(path)) {
            return UpdateBackuperCode.BackuperDoesNotExist;
        }

        var curr = BackuperInfo.Parse(await dbConnection.ReadAllLinesAsync(path));
        curr.Name = string.IsNullOrWhiteSpace(newName) ? curr.Name : newName;
        curr.MaxVersions = newMaxVersions <= 0 ? curr.MaxVersions : newMaxVersions;
        curr.UpdateOnBoot = newUpdateOnBoot ?? curr.UpdateOnBoot;

        var newValues = curr.ToStrings();
        await dbConnection.WriteAllLinesAsync(path, newValues);
        return UpdateBackuperCode.Success;
    }

    public DeleteBackuperCode DeleteBackuper(string name) {
        var path = GetBackuperPath(name);
        if(!dbConnection.Exists(path)) {
            return DeleteBackuperCode.BackuperDoesNotExist;
        }

        dbConnection.Delete(path);
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

