using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using System.Threading.Tasks;

namespace Backuper.UI.WPF.Commands;

public class DeleteBackuperCommand : AsyncCommandBase {

    private readonly INotificationService _notificationService;
    private readonly BackuperStore _backuperStore;
    private readonly BackuperViewModel _backuperVM;

    public DeleteBackuperCommand(BackuperViewModel backuperVM, BackuperStore backuperStore, INotificationService notificationService) {
        _backuperStore = backuperStore;
        _backuperVM = backuperVM;
        _notificationService = notificationService;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        var name = _backuperVM.Name;
        var result = await _backuperStore.DeleteBackuperAsync(name);

        var message = result switch {
            DeleteBackuperResponse.Success => $"The backuper {name} has been deleted successfully.",
            DeleteBackuperResponse.BackuperNotFound => $"The backuper {name} does not exist.",
            DeleteBackuperResponse.NameIsNullOrWhiteSpace => "The name cannot be empty.",
            _ => "There has been an unknown error.",
        };

        _notificationService.Send(message);

    }
}
