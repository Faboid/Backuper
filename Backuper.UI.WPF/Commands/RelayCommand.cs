using System;

namespace Backuper.UI.WPF.Commands;

public class RelayCommand : CommandBase {

    private readonly Action _action;

    public RelayCommand(Action action) {
        _action = action;
    }

    public override void Execute(object? parameter) => _action.Invoke();
}
