using Backuper.UI.WPF.Stores;
using System.Threading.Tasks;
using System.Windows;

namespace Backuper.UI.WPF.Commands; 

public class DeleteBackuperCommand : AsyncCommandBase {

    private readonly BackuperStore _backuperStore;

    public DeleteBackuperCommand(BackuperStore backuperStore) {
        _backuperStore = backuperStore;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        var name = parameter as string;
        var result = await _backuperStore.DeleteBackuperAsync(name!);

        var message = result switch {
            Core.Saves.DeleteBackuperCode.Success => $"The backuper {name} has been deleted successfully.",
            Core.Saves.DeleteBackuperCode.BackuperDoesNotExist => $"The backuper {name} does not exist.",
            _ => "There has been an unknown error.",
        };

        MessageBox.Show(message);

    }
}
