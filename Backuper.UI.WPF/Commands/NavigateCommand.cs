using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.ViewModels;

namespace Backuper.UI.WPF.Commands;

public class NavigateCommand<T> : LinkableCommandBase where T : ViewModelBase {

    private readonly NavigationService<T> _navigationService;
    private readonly bool _disposeCurrent;

    public NavigateCommand(bool disposeCurrent, NavigationService<T> navigationService) {
        _navigationService = navigationService;
        _disposeCurrent = disposeCurrent;
    }

    public NavigateCommand(bool disposeCurrent, NavigationService<T> navigationService, BusyService busyService) : base(busyService) {
        _navigationService = navigationService;
        _disposeCurrent = disposeCurrent;
    }

    public override void ExecuteLinked(object? parameter) {
        _navigationService.Navigate(_disposeCurrent);
    }
}
