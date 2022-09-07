using Backuper.UI.WPF.Services;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.Commands;

public abstract class AsyncCommandBase : LinkableCommandBase {

    public AsyncCommandBase() { }
    public AsyncCommandBase(BusyService busyService) : base(busyService) { }

    public override async void ExecuteLinked(object? parameter) {
        await ExecuteAsync(parameter);
    }

    protected abstract Task ExecuteAsync(object? parameter);

}
