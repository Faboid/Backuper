using Backuper.Core.Models;
using Backuper.Core.Saves.DBConnections;
using Backuper.Extensions;
using Backuper.Utils;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Backuper.Core.Tests")]
namespace Backuper.Core.Saves;

public class BackuperConnection {

    public BackuperConnection() : this(new FileDBConnection()) { }
    internal BackuperConnection(IDBConnection dbConnection) {
        this.dbConnection = dbConnection;
    }

    private readonly IDBConnection dbConnection;

    //todo - consider whether to use a wrapper to validate the given parameters
    //or to check them here

    public async Task<CreateBackuperCode> CreateBackuperAsync(BackuperInfo info) {
        if(string.IsNullOrWhiteSpace(info.Name)) {
            return CreateBackuperCode.NameNotValid;
        }

        if(dbConnection.Exists(info.Name)) {
            return CreateBackuperCode.BackuperExistsAlready;
        }
        var strings = info.ToStrings();
        await dbConnection.WriteAllLinesAsync(info.Name, strings);
        return CreateBackuperCode.Success;
    }

    public async Task<Option<BackuperInfo, GetBackuperCode>> GetBackuperAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return GetBackuperCode.NameNotValid;
        }

        if(!dbConnection.Exists(name)) {
            return GetBackuperCode.BackuperDoesNotExist;
        }
        var lines = await dbConnection.ReadAllLinesAsync(name);
        return BackuperInfo.Parse(lines);
    }

    public IAsyncEnumerable<BackuperInfo> GetAllBackupersAsync() {
        return dbConnection
            .EnumerateNames()
            .SelectAsync(x => dbConnection.ReadAllLinesAsync(x))
            .Select(x => BackuperInfo.Parse(x)); //todo - error handling in case the data of that backuper was corrupted
    }

    public async Task<UpdateBackuperCode> UpdateBackuperAsync(string name, string? newName = null, int newMaxVersions = 0, bool? newUpdateOnBoot = null) {
        if(string.IsNullOrWhiteSpace(name)) {
            return UpdateBackuperCode.NameNotValid;
        }

        if(!dbConnection.Exists(name)) {
            return UpdateBackuperCode.BackuperDoesNotExist;
        }

        var curr = BackuperInfo.Parse(await dbConnection.ReadAllLinesAsync(name));
        curr.Name = string.IsNullOrWhiteSpace(newName) ? curr.Name : newName;
        curr.MaxVersions = newMaxVersions <= 0 ? curr.MaxVersions : newMaxVersions;
        curr.UpdateOnBoot = newUpdateOnBoot ?? curr.UpdateOnBoot;

        var newValues = curr.ToStrings();
        await dbConnection.WriteAllLinesAsync(curr.Name, newValues);
        if(!string.IsNullOrWhiteSpace(newName)) {
            //if the name has changed, delete the old backuper file
            dbConnection.Delete(name);
        }

        return UpdateBackuperCode.Success;
    }

    public DeleteBackuperCode DeleteBackuper(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return DeleteBackuperCode.NameNotValid;
        }

        if(!dbConnection.Exists(name)) {
            return DeleteBackuperCode.BackuperDoesNotExist;
        }

        dbConnection.Delete(name);
        return DeleteBackuperCode.Success;
    }

    public enum CreateBackuperCode {
        Unknown,
        Success,
        Failure,
        NameNotValid,
        BackuperExistsAlready,
    }

    public enum GetBackuperCode {
        Unknown,
        BackuperDoesNotExist,
        NameNotValid,
    }

    public enum UpdateBackuperCode {
        Unknown,
        Success,
        BackuperDoesNotExist,
        NameNotValid
    }

    public enum DeleteBackuperCode {
        Unknown,
        Success,
        BackuperDoesNotExist,
        NameNotValid,
    }

}

