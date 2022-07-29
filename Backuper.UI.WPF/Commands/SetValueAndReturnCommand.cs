using Backuper.UI.WPF.Services;
using Backuper.UI.WPF.ViewModels;
using System;

namespace Backuper.UI.WPF.Commands; 

public class SetValueAndReturnCommand : CommandBase {

    private readonly OpenPathDialogViewModel _pathDialog;
    private readonly Action<string> _setter;
    private readonly NavigationService<ViewModelBase> _navigationService;

    public SetValueAndReturnCommand(OpenPathDialogViewModel pathDialog, NavigationService<ViewModelBase> navigationService, Action<string> setter) {
        _pathDialog = pathDialog;
        _setter = setter;
        _navigationService = navigationService;
    }

    public override void Execute(object? parameter) {
        _setter?.Invoke(_pathDialog.Path);
        _navigationService.Navigate();
    }
}
