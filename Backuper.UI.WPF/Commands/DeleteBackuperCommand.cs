using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace Backuper.UI.WPF.Commands; 

public class DeleteBackuperCommand : AsyncCommandBase {

    private readonly BackuperStore _backuperStore;
    private readonly BackuperViewModel _backuperVM;

    public DeleteBackuperCommand(BackuperViewModel backuperVM, BackuperStore backuperStore) {
        _backuperStore = backuperStore;
        _backuperVM = backuperVM;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        var name = _backuperVM.Name;
        var result = await _backuperStore.DeleteBackuperAsync(name);

        var message = result switch {
            Core.Saves.DeleteBackuperCode.Success => $"The backuper {name} has been deleted successfully.",
            Core.Saves.DeleteBackuperCode.BackuperDoesNotExist => $"The backuper {name} does not exist.",
            _ => "There has been an unknown error.",
        };

        MessageBox.Show(message);

    }
}
