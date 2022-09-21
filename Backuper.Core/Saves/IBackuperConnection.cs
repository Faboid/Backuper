using Backuper.Core.Models;
using Backuper.Utils;

namespace Backuper.Core.Saves; 

/// <summary>
/// Provides a set of methods to create, overwrite, or delete backupers.
/// </summary>
public interface IBackuperConnection {

    /// <summary>
    /// Determines whether the specified backuper exists.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Exists(string name);

    /// <summary>
    /// Creates a new backuper. If it exists already, it will get overwritten.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    Task SaveAsync(BackuperInfo info);

    /// <summary>
    /// Retrieves the backuper.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<BackuperInfo> GetAsync(string name);

    /// <summary>
    /// Returns a collection of all backupers. Corrupted backupers are returned as <see cref="Option{TValue, TError}"/>, proving only their name instead.
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<Option<BackuperInfo, string>> GetAllBackupersAsync();

    /// <summary>
    /// Edits an existing backuper. If the name is changed, the backuper will be migrated to its new name, and the old one gets deleted.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    Task OverwriteAsync(string name, BackuperInfo info);

    /// <summary>
    /// Deletes a backuper.
    /// </summary>
    /// <param name="name"></param>
    void Delete(string name);

}
