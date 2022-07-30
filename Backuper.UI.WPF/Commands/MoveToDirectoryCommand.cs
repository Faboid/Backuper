using Backuper.UI.WPF.ViewModels;
using System;
using System.IO;

namespace Backuper.UI.WPF.Commands; 

public class MoveToDirectoryCommand : CommandBase {

    private readonly OpenPathDialogViewModel _openPathDialogViewModel;

    public MoveToDirectoryCommand(OpenPathDialogViewModel openPathDialogViewModel) {
        _openPathDialogViewModel = openPathDialogViewModel;
    }

    public override void Execute(object? parameter) {

        if(parameter is not DirectoryInfo newDir) {
            return;
        }

        _openPathDialogViewModel.CurrentPath = newDir.FullName;

    }
}
