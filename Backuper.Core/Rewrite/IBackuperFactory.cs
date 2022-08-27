using Backuper.Core.Models;
using Backuper.Utils;

namespace Backuper.Core.Rewrite;
public interface IBackuperFactory {
    Option<IBackuper, BackuperFactory.CreateBackuperFailureCode> CreateBackuper(BackuperInfo info);
    IAsyncEnumerable<IBackuper> LoadBackupers();
}