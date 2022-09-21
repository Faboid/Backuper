using Backuper.Core.Models;
using Backuper.Utils;

namespace Backuper.Core;

/// <summary>
/// Provides a set of methods to create a <see cref="IBackuper"/>
/// </summary>
public interface IBackuperFactory {

    /// <summary>
    /// Tries to initialize an instance of <see cref="IBackuper"/> with the given <paramref name="info"/>.
    /// If successful, the backuper's data will be saved for future sessions.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    Task<Option<IBackuper, BackuperFactory.CreateBackuperFailureCode>> CreateBackuper(BackuperInfo info);

    /// <summary>
    /// Loads all <see cref="IBackuper"/> saved previously.
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<IBackuper> LoadBackupers();
}