using Backuper.Core.Models;
using Backuper.Core.Saves;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace Backuper.UI.WPF.Commands; 

public class CreateBackuperCommand : AsyncCommandBase {

    private readonly CreateBackuperViewModel createBackuperViewModel;
    private readonly BackuperStore backuperStore;

    public CreateBackuperCommand(CreateBackuperViewModel createBackuperViewModel, BackuperStore backuperStore) {
        this.createBackuperViewModel = createBackuperViewModel;
        this.backuperStore = backuperStore;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        BackuperInfo info = new(
                createBackuperViewModel.BackuperName,
                createBackuperViewModel.SourcePath,
                createBackuperViewModel.MaxVersions,
                createBackuperViewModel.UpdateOnBoot
            );

        var result = await backuperStore.CreateBackuperAsync(info);

        var message = result switch {
            CreateBackuperCode.Success => "The backuper has been created.",
            CreateBackuperCode.NameNotValid => "The given name cannot be empty.",
            CreateBackuperCode.BackuperExistsAlready => "The given name is used by another backuper.",
            CreateBackuperCode.SourceDoesNotExist => "The given source path does not exist.",
            _ => "There has been an unknown error."
        };

        //todo - show the result in a more graceful way
        MessageBox.Show(message);

    }
}
