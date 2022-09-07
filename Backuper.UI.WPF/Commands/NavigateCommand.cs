using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.ViewModels;

namespace Backuper.UI.WPF.Commands;

public class NavigateCommand<T> : LinkableCommandBase where T : ViewModelBase {

    private readonly NavigationService<T> navigationService;

    public NavigateCommand(NavigationService<T> navigationService) {
        this.navigationService = navigationService;
    }

    public NavigateCommand(NavigationService<T> navigationService, BusyService busyService) : base(busyService) {
        this.navigationService = navigationService;
    }

    public override void ExecuteLinked(object? parameter) {
        navigationService.Navigate();
    }
}
