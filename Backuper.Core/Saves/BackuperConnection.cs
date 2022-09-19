using Backuper.Core.Models;
using Backuper.Core.Saves.DBConnections;
using Backuper.Utils;

namespace Backuper.Core.Saves;

public class BackuperConnection : IBackuperConnection {

    public BackuperConnection(PathsHandler pathsHandler) : this(new FileDBConnection(pathsHandler)) { }
    internal BackuperConnection(IDBConnection dbConnection) {
        this.dbConnection = dbConnection;
    }

    private readonly IDBConnection dbConnection;

    public bool Exists(string name) {
        return dbConnection.Exists(name);
    }

    public async Task SaveAsync(BackuperInfo info) {
        var strings = info.ToStrings();
        await dbConnection.WriteAllLinesAsync(info.Name, strings);
    }

    public async Task<BackuperInfo> GetAsync(string name) {
        var lines = await dbConnection.ReadAllLinesAsync(name);
        return BackuperInfo.Parse(lines);
    }

    //Option<Info, name>
    public async IAsyncEnumerable<Option<BackuperInfo, string>> GetAllBackupersAsync() {
        var names = dbConnection.EnumerateNames();
        foreach (var name in names) {

            var lines = await dbConnection.ReadAllLinesAsync(name);
            var info = BackuperInfo.TryParse(lines).Or(null);
            if(info != null) {
                yield return info;
            } else {
                yield return name;
            }

        }
    }

    public async Task OverwriteAsync(string name, BackuperInfo info) {

        await SaveAsync(info);

        if(name != info.Name) {
            Delete(name);
        }
    }

    public void Delete(string name) {
        dbConnection.Delete(name);
    }

}
