using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.ViewModels;

namespace Backuper.UI.WPF.Commands;

public class NavigateCommand<T> : CommandBase where T: ViewModelBase {

    private readonly NavigationService<T> navigationService;

    public NavigateCommand(NavigationService<T> navigationService) {
        this.navigationService = navigationService;
    }

    public override void Execute(object? parameter) {
        navigationService.Navigate();
    }
}
