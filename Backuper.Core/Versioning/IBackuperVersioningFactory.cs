namespace Backuper.Core.Versioning;

/// <summary>
/// Provides a set of methods to create a <see cref="IBackuperVersioning"/>
/// </summary>
public interface IBackuperVersioningFactory {

    /// <summary>
    /// Initializes an instance of <see cref="IBackuperVersioning"/> on the specified name.
    /// </summary>
    /// <param name="backuperName"></param>
    /// <returns></returns>
    IBackuperVersioning CreateVersioning(string backuperName);
}