using System.Threading.Tasks;

namespace Backuper.UI.WPF.Commands;

public abstract class AsyncCommandBase : CommandBase {

    private bool _isBusy = false;

    public bool IsBusy {
        get { return _isBusy; }
        set {
            _isBusy = value;
            OnCanExecuteChanged();
        }
    }

    public override bool CanExecute(object? parameter) {
        return !IsBusy && base.CanExecute(parameter);
    }

    public override async void Execute(object? parameter) {

        IsBusy = true;

        try {

            await ExecuteAsync(parameter);
        } finally {

            IsBusy = false;
        }

    }

    protected abstract Task ExecuteAsync(object? parameter);

}
