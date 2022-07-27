using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using System;

namespace Backuper.UI.WPF.Commands;

public class NavigateCommand<T> : CommandBase where T: ViewModelBase {

    private readonly NavigationStore _navigationStore;
    private readonly Func<T> _navigationFunction;

    public NavigateCommand(NavigationStore navigationStore, Func<T> navigationFunction) {
        _navigationStore = navigationStore;
        _navigationFunction = navigationFunction;
    }

    public override void Execute(object? parameter) {
        _navigationStore.CurrentViewModel = _navigationFunction.Invoke();
    }
}
