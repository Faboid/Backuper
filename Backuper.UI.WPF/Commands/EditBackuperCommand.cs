using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.Commands;

public class EditBackuperCommand : AsyncCommandBase {

    private readonly INotificationService _notificationService;
    private readonly EditBackuperViewModel _editBackuperViewModel;
    private readonly BackuperStore _backuperStore;
    private readonly NavigationService<BackuperListingViewModel> _navigatorToBackuperListingViewModel;

    public EditBackuperCommand(EditBackuperViewModel editBackuperViewModel, BackuperStore backuperStore, INotificationService notificationService, NavigationService<BackuperListingViewModel> navigatorToBackuperListingViewModel) {
        _editBackuperViewModel = editBackuperViewModel;
        _backuperStore = backuperStore;
        _navigatorToBackuperListingViewModel = navigatorToBackuperListingViewModel;
        _notificationService = notificationService;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        var result = await _backuperStore.UpdateBackuperAsync(
            _editBackuperViewModel.PreviousName,
            _editBackuperViewModel.BackuperName,
            _editBackuperViewModel.MaxVersions,
            _editBackuperViewModel.UpdateOnBoot
            );

        var newName = _editBackuperViewModel.BackuperName ?? _editBackuperViewModel.PreviousName;

        var message = result switch {
            UpdateBackuperResponse.Success => $"{newName} has been edited successfully.",
            UpdateBackuperResponse.BackuperNotFound => $"{_editBackuperViewModel.PreviousName} has not been found.",
            UpdateBackuperResponse.NewMaxVersionsIsZeroOrNegative => "The max versions cannot be less than one.",
            UpdateBackuperResponse.NewNameIsOccupied => $"{_editBackuperViewModel.BackuperName} is already in use.",
            _ => "An error has occurred when trying to edit the backuper.",
        };

        _notificationService.Send(message, "Result");

        if(result == UpdateBackuperResponse.Success) {
            _navigatorToBackuperListingViewModel.Navigate();
        }

    }
}
