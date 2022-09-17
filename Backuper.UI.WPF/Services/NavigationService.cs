using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using Serilog;
using System;

namespace Backuper.UI.WPF.Services;

public class NavigationService<T> where T : ViewModelBase {

    private readonly ILogger _logger = Log.Logger;
    private readonly NavigationStore _navigationStore;
    private readonly Func<T> _navigationFunction;

    public NavigationService(NavigationStore navigationStore, Func<T> navigationFunction) {
        _navigationStore = navigationStore;
        _navigationFunction = navigationFunction;
    }

    public void Navigate(bool disposeCurrent) {
        if(disposeCurrent) {
            _navigationStore.CurrentViewModel?.Dispose();
        }
        var newVM = _navigationFunction.Invoke();
        _logger.Debug("Navigating from {Current}, to {New}", _navigationStore.CurrentViewModel?.GetType().Name, newVM.GetType().Name);
        _navigationStore.CurrentViewModel = newVM;
    }

}
