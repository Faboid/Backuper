using Backuper.Core.Saves.DBConnections;
namespace Backuper.Core.Saves;

public class ConnectionFactory {

    public static IBackuperConnection CreateConnection(BackupType type) {

        IDBConnection conn = type switch {
            BackupType.Memory => new MemoryDBConnection(),
            BackupType.Text => new FileDBConnection(),
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

        var backuperConn = new BackuperConnection(conn);
        return backuperConn;
    }

    public enum BackupType {
        Memory,
        Text,
    }

}
