using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using System;

namespace Backuper.UI.WPF.Services;

public class NavigationService<T> where T : ViewModelBase {

    private readonly NavigationStore _navigationStore;
    private readonly Func<T> _navigationFunction;

    public NavigationService(NavigationStore navigationStore, Func<T> navigationFunction) {
        _navigationStore = navigationStore;
        _navigationFunction = navigationFunction;
    }

    public void Navigate() {
        _navigationStore.CurrentViewModel = _navigationFunction.Invoke();
    }

}
