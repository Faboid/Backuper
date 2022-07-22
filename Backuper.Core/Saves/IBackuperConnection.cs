using Backuper.Core.Models;
using Backuper.Utils;
namespace Backuper.Core.Saves;

/// <summary>
/// Provides methods to save backupers between sessions.
/// </summary>
public interface IBackuperConnection {

    Task<CreateBackuperCode> CreateBackuperAsync(BackuperInfo info);
    Task<Option<BackuperInfo, GetBackuperCode>> GetBackuperAsync(string name);
    IAsyncEnumerable<BackuperInfo> GetAllBackupersAsync();
    Task<UpdateBackuperCode> UpdateBackuperAsync(string name, string? newName = null, int newMaxVersions = 0, bool? newUpdateOnBoot = null);
    DeleteBackuperCode DeleteBackuper(string name);

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