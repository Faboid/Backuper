using Backuper.Core;
using Backuper.Core.Saves;
using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.Stores;
using Backuper.UI.WPF.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace Backuper.UI.WPF.Commands; 

public class EditBackuperCommand : AsyncCommandBase {

    private readonly EditBackuperViewModel _editBackuperViewModel;
    private readonly IBackuper _backuper;
    private readonly BackuperStore _backuperStore;
    private readonly NavigationService<BackuperListingViewModel> _navigatorToBackuperListingViewModel;

    public EditBackuperCommand(IBackuper backuper, EditBackuperViewModel editBackuperViewModel, BackuperStore backuperStore, NavigationService<BackuperListingViewModel> navigatorToBackuperListingViewModel) {
        _backuper = backuper;
        _editBackuperViewModel = editBackuperViewModel;
        _backuperStore = backuperStore;
        _navigatorToBackuperListingViewModel = navigatorToBackuperListingViewModel;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        var result = await _backuperStore.UpdateBackuperAsync(
            _editBackuperViewModel.PreviousName,
            _editBackuperViewModel.BackuperName, 
            _editBackuperViewModel.MaxVersions, 
            _editBackuperViewModel.UpdateOnBoot
            );

        //migrate existing backups
        if(result == UpdateBackuperCode.Success) {
            await _backuper.EditBackuperAsync(
                _editBackuperViewModel.BackuperName,
                _editBackuperViewModel.MaxVersions,
                _editBackuperViewModel.UpdateOnBoot
                );
        }

        var message = result switch {
            UpdateBackuperCode.Success => "The backuper has been edited successfully.",
            UpdateBackuperCode.BackuperDoesNotExist => "The backuper you're trying to edit does not exist.",
            UpdateBackuperCode.NameNotValid => "The new name is not valid.",
            _ => "An unknown error has occurred when trying to edit the backuper.",
        };

        MessageBox.Show(message, "Edit Result");

        if(result == UpdateBackuperCode.Success) { 
            _navigatorToBackuperListingViewModel.Navigate();
        }

    }
}
