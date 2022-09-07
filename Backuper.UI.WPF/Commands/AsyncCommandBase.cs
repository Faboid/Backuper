using Backuper.UI.WPF.Services;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.Commands;

public abstract class AsyncCommandBase : CommandBase {

    public AsyncCommandBase() : this(new()) { }

    public AsyncCommandBase(BusyService busyService) {
        _busyService = busyService;
        _busyService.BusyChanged += OnCanExecuteChanged;
    }

    private readonly BusyService _busyService;

    public override bool CanExecute(object? parameter) {
        return _busyService.IsFree && base.CanExecute(parameter);
    }

    public override async void Execute(object? parameter) {
        using var busy = _busyService.GetBusy();
        await ExecuteAsync(parameter);
    }

    protected abstract Task ExecuteAsync(object? parameter);

}
