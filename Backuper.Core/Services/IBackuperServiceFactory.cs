namespace Backuper.Core.Services;

/// <summary>
/// Provides a set of methods to create a <see cref="IBackuperService"/>.
/// </summary>
public interface IBackuperServiceFactory {

    /// <summary>
    /// Instances a specific <see cref="IBackuperService"/> depending on the given path.
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <returns></returns>
    IBackuperService CreateBackuperService(string sourcePath);

    /// <summary>
    /// Instances a corrupted service. This is reserved for backupers whose data is corrupted.
    /// </summary>
    /// <returns></returns>
    IBackuperService CreateCorruptedService();
}

