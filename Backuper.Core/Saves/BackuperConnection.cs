using Backuper.Core.Models;
using Backuper.Core.Saves.DBConnections;
using Backuper.Extensions;

namespace Backuper.Core.Saves;

public class BackuperConnection : IBackuperConnection
{

    public BackuperConnection() : this(new FileDBConnection()) { }
    internal BackuperConnection(IDBConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    private readonly IDBConnection dbConnection;

    public bool Exists(string name)
    {
        return dbConnection.Exists(name);
    }

    public async Task SaveAsync(BackuperInfo info)
    {
        var strings = info.ToStrings();
        await dbConnection.WriteAllLinesAsync(info.Name, strings);
    }

    public async Task<BackuperInfo> GetAsync(string name)
    {
        var lines = await dbConnection.ReadAllLinesAsync(name);
        return BackuperInfo.Parse(lines);
    }

    public IAsyncEnumerable<BackuperInfo> GetAllBackupersAsync()
    {
        return dbConnection
            .EnumerateNames()
            .SelectAsync(x => dbConnection.ReadAllLinesAsync(x))
            .Select(x => BackuperInfo.Parse(x)); //todo - error handling in case the data of that backuper was corrupted
    }

    public async Task OverwriteAsync(string name, BackuperInfo info)
    {

        await SaveAsync(info);

        if (name != info.Name)
        {
            Delete(name);
        }
    }

    public void Delete(string name)
    {
        dbConnection.Delete(name);
    }

}
