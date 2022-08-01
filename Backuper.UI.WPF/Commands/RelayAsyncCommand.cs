using System;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.Commands; 

public class AsyncRelayCommand : AsyncCommandBase {

    private readonly Func<Task> _callback;

    public AsyncRelayCommand(Func<Task> callback) {
        _callback = callback;
    }

    protected override Task ExecuteAsync(object? parameter) => _callback.Invoke();

}

public class AsyncRelayCommand<T> : AsyncCommandBase {

    private readonly Func<T, Task> _callback;

    public AsyncRelayCommand(Func<T, Task> callback) {
        _callback = callback;
    }

    public override bool CanExecute(object? parameter) {
        return (parameter == null || parameter is T) && base.CanExecute(parameter);
    }

    protected override Task ExecuteAsync(object? parameter) => _callback?.Invoke((T)parameter!)!;
}
