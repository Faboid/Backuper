using Backuper.Core.Models;
using Backuper.Utils;

namespace Backuper.Core;
public interface IBackuperFactory
{
    Task<Option<IBackuper, BackuperFactory.CreateBackuperFailureCode>> CreateBackuper(BackuperInfo info);
    IAsyncEnumerable<IBackuper> LoadBackupers();
}